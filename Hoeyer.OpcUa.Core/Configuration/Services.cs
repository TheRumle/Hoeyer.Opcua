using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Core.Entity.State;
using Hoeyer.OpcUa.Core.Extensions.Loading;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Configuration;

public static class Services
{
    public static OnGoingOpcEntityServiceRegistration WithEntityServices(
        this OnGoingOpcEntityServiceRegistration registration)
    {
        var services = registration.Collection;

        var types = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .ToList();

        var startupLoaders = types.GetEntityServicesOfType(typeof(IEntityStartupLoader<>)).ToArray();
        var nodeFactories = types.GetEntityServicesOfType(typeof(IEntityNodeFactory<>)).ToArray();
        var translators = types.GetEntityServicesOfType(typeof(IEntityTranslator<>)).ToArray();


        var tuples = (
            from startup in startupLoaders
            join factory in nodeFactories on startup.Entity equals factory.Entity
            join translator in translators on startup.Entity equals translator.Entity
            select (
                startup: startup.ToServiceDescriptor(ServiceLifetime.Scoped),
                nodeFactory: factory.ToServiceDescriptor(ServiceLifetime.Transient),
                translator: translator.ToServiceDescriptor(ServiceLifetime.Transient),
                entity: startup.Entity)
        ).ToArray();

        var entityMatches = tuples.Select(t => t.entity).ToArray();
        List<OpcUaConfigurationException> exceptions =
        [
            ..ThrowIfServiceMissingFrom(entityMatches, startupLoaders),
            ..ThrowIfServiceMissingFrom(entityMatches, nodeFactories),
            ..ThrowIfServiceMissingFrom(entityMatches, translators)
        ];
        if (exceptions.Any()) throw new OpcUaConfigurationException(exceptions);

        foreach (var loader in startupLoaders)
            services.AddSingleton(loader.ConcreteServiceType, loader.ImplementationType);

        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            foreach (var (startupLoader, nodeFactory, _, entityType) in tuples)
            {
                var entityInstance = GetEntityInstance(scope, startupLoader);
                services.AddSingleton(entityType, entityInstance);
                services.AddSingleton(nodeFactory.ServiceType, nodeFactory.ImplementationType!);
                services.AddSingleton(nodeFactory.ImplementationType!, nodeFactory.ImplementationType!);
                services.Remove(startupLoader); //Once the project is started, the startup should not be available!
            }

            services.AddTransient<IEnumerable<IEntityNodeFactory>>(p =>
                nodeFactories.Select(value => p.GetService(value.ConcreteServiceType) as IEntityNodeFactory)!
            );
        }

        return registration;
    }

    private static object GetEntityInstance(IServiceScope scope, ServiceDescriptor startupLoader)
    {
        var startupLoaderInstance = scope.ServiceProvider.GetRequiredService(startupLoader.ServiceType);
        var startupLoaderType = startupLoader.ServiceType;
        var entityInstance = startupLoaderType
            .GetMethod(nameof(IEntityStartupLoader<object>.LoadStartUpState))!
            .Invoke(startupLoaderInstance, null);
        return entityInstance;
    }


    private static IEnumerable<OpcUaConfigurationException> ThrowIfServiceMissingFrom(IEnumerable<Type> matchedServices,
        EntityServiceContext[] services)
    {
        return services
            .Where(s => !matchedServices.Contains(s.Entity))
            .Select(e => OpcUaConfigurationException.ServiceNotConfigured(e.Entity, e.ServiceType));
    }

    public static OnGoingOpcEntityServiceRegistration AddOpcUaServerConfiguration(this IServiceCollection services,
        Func<IEntityServerConfigurationBuilder, IOpcUaEntityServerInfo> configurationBuilder)
    {
        var entityServerConfiguration = configurationBuilder.Invoke(EntityServerConfigurationBuilder.Create());
        services.AddSingleton(entityServerConfiguration);
        return new OnGoingOpcEntityServiceRegistration(services);
    }
}