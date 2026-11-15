using System.Collections.Frozen;
using System.Reflection;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Application.NodeStructure;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.Configuration;
using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;
using Opc.Ua.Security.Certificates;

namespace Hoeyer.OpcUa.Server.Services;

public static class ServiceExtensions
{
    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistrationWithModels serviceRegistration,
        Type fromAssembly,
        Action<IServiceProvider, ServerConfigurable>? additionalConfiguration = null)
        =>
            serviceRegistration.WithOpcUaServer([fromAssembly.Assembly],
                additionalConfiguration
            );

    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistrationWithModels serviceRegistration,
        IEnumerable<Type> assembliesContainingLoaders,
        Action<IServiceProvider, ServerConfigurable>? additionalConfiguration = null)
        => serviceRegistration.WithOpcUaServer(assembliesContainingLoaders.Select(e => e.Assembly),
            additionalConfiguration
        );

    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistrationWithModels serviceRegistration,
        IEnumerable<Assembly> assembliesContainingLoaders,
        Action<IServiceProvider, ServerConfigurable>? additionalConfiguration = null)
    {
        var collection = serviceRegistration.Collection;
        AddLoaders(serviceRegistration.Collection, serviceRegistration.EntityCollection, assembliesContainingLoaders);

        collection
            .AddSingleton<ICertificateFactory, DefaultCertificateFactory>()
            .AddSingleton(typeof(IEntityNodeStructureFactory<>), typeof(ReflectionBasedEntityStructureFactory<>))
            .AddServiceAndImplSingleton<IOpcUaTargetServerSetup, OpcUaTargetServerSetup>()
            .AddSingleton<IServerApplicationConfigurationFactory, ServerApplicationConfigurationFactory>()
            .AddSingleton<ServerApplicationConfigurationAction>(serviceProvider =>
            {
                return config =>
                {
                    config.OpcUaApplicationConfiguration.ServerConfiguration ??= new ServerConfiguration();
                    additionalConfiguration?.Invoke(serviceProvider, config.GetServerConfigurable());
                };
            });

        var registration = typeof(ServiceExtensions).CreateStaticMethodInvoker(nameof(AddServices), collection);
        foreach (var entity in serviceRegistration.EntityCollection.ModelledEntities
                     .Where(type => type.IsAnnotatedWith<OpcUaEntityAttribute>())
                     .ToFrozenSet())
        {
            registration.Invoke(entity);
        }

        collection
            .AddTransient<ServerApplicationConfiguration>(provider =>
                provider.GetRequiredService<IServerApplicationConfigurationFactory>().CreateServerConfiguration()
            )
            .AddSingleton<ServerApplication>()
            .AddServiceAndImplSingleton(typeof(INodeConfigurator<>), typeof(AlarmSetupConfigurator<>))
            .AddServiceAndImplSingleton(typeof(INodeConfigurator<>), typeof(AlarmLoggingConfigurator<>))
            .AddServiceAndImplSingleton<IEntityNodeAccessConfigurator, NoAccessRestrictionsConfigurator>()
            .AddServiceAndImplSingleton<IServerStartedHealthCheck, HealthCheck>()
            .AddSingleton<IHealthCheckAssignment>(p => p.GetRequiredService<HealthCheck>())
            .AddSingleton<IOpcEntityServer, OpcEntityServer>()
            .AddSingleton<IStartableEntityServer, StartableEntityServer>();

        return new OnGoingOpcEntityServerServiceRegistration(serviceRegistration.Collection);
    }

    private static void AddServices<TEntity>(IServiceCollection collection)
    {
        collection
            .AddSingleton<IManagedEntityNodeProvider<TEntity>, ManagedEntityNodeProvider<TEntity>>()
            .AddSingleton<EntityNodeManagerSingletonFactory<TEntity>>()
            .AddSingleton<IEntityNodeManagerFactory<TEntity>>(sp =>
                sp.GetRequiredService<EntityNodeManagerSingletonFactory<TEntity>>())
            .AddSingleton<IEntityNodeManagerFactory>(sp =>
                sp.GetRequiredService<EntityNodeManagerSingletonFactory<TEntity>>())
            .AddSingleton<IEntityManagerHolder<TEntity>>(sp =>
                sp.GetRequiredService<EntityNodeManagerSingletonFactory<TEntity>>())
            .AddSingleton<IEntityManagerHolder>(sp =>
                sp.GetRequiredService<IEntityManagerHolder<TEntity>>()
            );
    }


    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServerAsBackgroundService(
        this OnGoingOpcEntityServiceRegistrationWithModels serviceRegistration,
        Type assemblyMarker,
        Action<IServiceProvider, ServerConfigurable>? additionalConfiguration = null
    )
    {
        var serverConfig = serviceRegistration.WithOpcUaServer(
            [assemblyMarker.Assembly],
            additionalConfiguration
        );
        serverConfig.Collection.AddHostedService<OpcUaServerBackgroundService>();
        return serverConfig;
    }

    private static void AddLoaders(IServiceCollection collection, EntityTypesCollection types,
        IEnumerable<Assembly> assemblies)
    {
        var wantedInterfaces = types
            .ModelledEntities
            .Select(entity => typeof(IEntityLoader<>).MakeGenericType(entity))
            .ToHashSet();

        var loaders = assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsInterface: false, IsAbstract: false })
            .Select(type =>
            {
                var implementedInterface = wantedInterfaces.FirstOrDefault(e => e.IsAssignableFrom(type));
                if (implementedInterface is not null)
                {
                    return (Service: implementedInterface, Implementation: type);
                }

                return default;
            })
            .Where(result => result.Service is not null);

        foreach (var (service, implementation) in loaders)
        {
            collection.AddServiceAndImplSingleton(service, implementation);
        }
    }
}