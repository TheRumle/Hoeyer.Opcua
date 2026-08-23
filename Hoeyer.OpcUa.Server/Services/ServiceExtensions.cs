using System.Collections.Frozen;
using System.Reflection;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Application.NodeStructure;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.Configuration;
using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Services;

public static class ServiceKeys
{
    public const string CONFIGURATION_KEY = "SERVER_APPLICATION_CONFIGURATION";
}

public static class ServiceExtensions
{
    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistrationWithModels serviceRegistration,
        Type fromAssembly,
        Action<IServiceProvider, ServerConfiguration>? additionalConfiguration = null)
        =>
            serviceRegistration.WithOpcUaServer([fromAssembly.Assembly],
                additionalConfiguration
            );

    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistrationWithModels serviceRegistration,
        IEnumerable<Type> assembliesContainingLoaders,
        Action<IServiceProvider, ServerConfiguration>? additionalConfiguration = null)
        => serviceRegistration.WithOpcUaServer(assembliesContainingLoaders.Select(e => e.Assembly),
            additionalConfiguration
        );

    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistrationWithModels serviceRegistration,
        IEnumerable<Assembly> assembliesContainingLoaders,
        Action<IServiceProvider, ServerConfiguration>? additionalConfiguration = null)
    {
        var collection = serviceRegistration.Collection;

        collection.AddSingleton(typeof(IEntityNodeStructureFactory<>), typeof(ReflectionBasedEntityStructureFactory<>));
        collection.AddServiceAndImplSingleton<IOpcUaTargetServerSetup, OpcUaTargetServerSetup>();
        collection.AddSingleton<IServerApplicationConfigurationFactory, ServerApplicationConfigurationFactory>();
        collection.AddSingleton<ServerApplicationConfigurationAction>(serviceProvider =>
        {
            return config =>
            {
                config.ServerConfiguration ??= new ServerConfiguration();
                additionalConfiguration?.Invoke(serviceProvider, config.ServerConfiguration);
            };
        });

        var registration = typeof(ServiceExtensions)
            .CreateStaticMethodInvoker(nameof(AddServices), collection);

        foreach (var entity in serviceRegistration.EntityCollection.ModelledEntities
                     .Where(type => type.IsAnnotatedWith<OpcUaEntityAttribute>())
                     .ToFrozenSet())
        {
            registration.Invoke(entity);
        }

        collection.AddServiceAndImplSingleton(typeof(INodeConfigurator<>), typeof(AlarmSetupConfigurator<>));
        collection.AddServiceAndImplSingleton(typeof(INodeConfigurator<>), typeof(AlarmLoggingConfigurator<>));
        collection.AddServiceAndImplSingleton<IEntityNodeAccessConfigurator, NoAccessRestrictionsConfigurator>();
        collection.AddServiceAndImplSingleton<IServerStartedHealthCheck, HealthCheck>();
        collection.AddSingleton<IHealthCheckAssignment>(p => p.GetRequiredService<HealthCheck>());
        collection.AddServiceAndImplSingleton<IOpcUaEntityServerFactory, OpcUaEntityServerFactory>();
        collection.AddSingleton<IStartableEntityServer>(p =>
            p.GetRequiredService<IOpcUaEntityServerFactory>().CreateServer());
        collection.AddSingleton<OpcEntityServer>();
        AddLoaders(serviceRegistration.Collection, assembliesContainingLoaders);
        return new OnGoingOpcEntityServerServiceRegistration(serviceRegistration.Collection);
    }

    private static void AddServices<TEntity>(IServiceCollection collection)
    {
        collection
            .AddServiceAndImplSingleton<IManagedEntityNodeProvider<TEntity>, ManagedEntityNodeProvider<TEntity>>();
        collection
            .AddServiceAndImplSingleton<IEntityNodeManagerFactory<TEntity>,
                EntityNodeManagerSingletonFactory<TEntity>>();
        collection.AddServiceAndImplSingleton(typeof(IEntityNodeManagerFactory),
            typeof(EntityNodeManagerSingletonFactory<TEntity>));

        collection
            .AddServiceAndImplSingleton<IMaybeInitializedEntityManager<TEntity>,
                MaybeInitializedEntityManager<TEntity>>();
        collection.AddSingleton(typeof(IMaybeInitializedEntityManager), typeof(MaybeInitializedEntityManager<TEntity>));
    }

    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServerAsBackgroundService(
        this OnGoingOpcEntityServiceRegistrationWithModels serviceRegistration,
        Type assemblyMarker,
        Action<IServiceProvider, ServerConfiguration>? additionalConfiguration = null
    )
    {
        var serverConfig = serviceRegistration.WithOpcUaServer(
            [assemblyMarker.Assembly],
            additionalConfiguration
        );
        serverConfig.Collection.AddHostedService<OpcUaServerBackgroundService>();
        return serverConfig;
    }

    private static void AddLoaders(IServiceCollection collection, IEnumerable<Assembly> assemblies)
    {
        var loaderType = typeof(IEntityLoader<>);
        var loaders = assemblies.SelectMany(assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t != null).ToArray();
                }
            })
            .Where(type => type is { IsInterface: false, IsAbstract: false })
            .Select(type =>
            {
                var foundLoaderInterface = type
                    .GetInterfaces()
                    .FirstOrDefault(@interface => @interface.Namespace == loaderType.Namespace
                                                  && @interface.IsConstructedGenericType &&
                                                  @interface.GetGenericTypeDefinition() == loaderType);

                if (foundLoaderInterface is null)
                {
                    return default;
                }

                return (Service: foundLoaderInterface, Implementation: type);
            })
            .Where(result => result.Service is not null);

        foreach (var (service, implementation) in loaders)
        {
            collection.AddServiceAndImplSingleton(service, implementation);
        }
    }
}