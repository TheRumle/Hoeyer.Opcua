﻿using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Core.Extensions.Loading;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Configuration;

public static class Services
{
    public static OnGoingOpcEntityServiceRegistration AddOpcUaServerConfiguration(this IServiceCollection services,
        Func<IEntityServerConfigurationBuilder, IOpcUaEntityServerInfo> configurationBuilder)
    {
        var entityServerConfiguration = configurationBuilder.Invoke(EntityServerConfigurationBuilder.Create());
        services.AddSingleton(entityServerConfiguration);
        return new OnGoingOpcEntityServiceRegistration(services);
    }
    
    public static OnGoingOpcEntityServiceRegistration WithEntityServices(
        this OnGoingOpcEntityServiceRegistration registration)      
    {
        var services = registration.Collection;

        var types = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .ToList();

        var startupLoaders = types.GetEntityServicesOfType(typeof(IEntityLoader<>)).ToArray();
        var nodeFactories = types.GetEntityServicesOfType(typeof(IEntityNodeStructureFactory<>)).ToArray();
        var translators = types.GetEntityServicesOfType(typeof(IEntityTranslator<>)).ToArray();

        var serviceTriple = (
            from startup in startupLoaders
            join factory in nodeFactories on startup.Entity equals factory.Entity
            join translator in translators on startup.Entity equals translator.Entity
            select (startup, factory, translator)
        ).ToArray();

        var entityTypes = serviceTriple.Select(t => t.factory.Entity).ToArray();
        List<OpcUaServiceConfigurationException> exceptions =
        [
            ..GetErrorsIfServiceMissingFrom(entityTypes, startupLoaders),
            ..GetErrorsIfServiceMissingFrom(entityTypes, nodeFactories),
            ..GetErrorsIfServiceMissingFrom(entityTypes, translators)
        ];
        if (exceptions.Any()) throw new OpcUaServiceConfigurationException(exceptions);

        AddCoreServices(serviceTriple, services);
        AddInitializerServices(entityTypes, services);



        return registration;
    }

    private static void AddInitializerServices(Type[] entityTypes, IServiceCollection services)
    {
        var initializerServices = entityTypes.Select(entity => typeof(EntityInitializer<object>)
                .GetGenericTypeDefinition()
                .MakeGenericType(entity))
            .ToList();

        foreach (var initializerService in initializerServices)
        {
            services.AddTransient(initializerService, initializerService);
        }
        
        services.AddTransient<IEnumerable<IEntityInitializer>>(p =>
            initializerServices.Select(initializerService 
                => p.GetService(initializerService) as IEntityInitializer 
                                                             ?? throw new OpcUaServiceConfigurationException($"Trying to register {initializerService.Name} as an  {nameof(IEntityInitializer)}, but this is not possible."))
        );
    }

    private static void AddCoreServices(
        (EntityServiceContext startup, EntityServiceContext factory, EntityServiceContext translator)[] serviceTriple,
        IServiceCollection services)
    {
        foreach (var (startupLoader, nodeFactory, translator)  in serviceTriple)
        {
            services.AddTransient(translator.ConcreteServiceType, translator.ImplementationType);
            services.AddSingleton(startupLoader.ConcreteServiceType, startupLoader.ImplementationType);
            services.AddSingleton(nodeFactory.ConcreteServiceType, nodeFactory.ImplementationType);
        }
    }


    private static IEnumerable<OpcUaServiceConfigurationException> GetErrorsIfServiceMissingFrom(IEnumerable<Type> matchedServices,
        EntityServiceContext[] services)
    {
        return services
            .Where(s => !matchedServices.Contains(s.Entity))
            .Select(e => OpcUaServiceConfigurationException.ServiceNotConfigured(e.Entity, e.ServiceType));
    }


}