using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructure;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Services;

public static class ServiceExtensions
{
    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistration serviceRegistration,
        Type fromAssembly,
        Action<ServerConfiguration>? additionalConfiguration = null)
        => WithOpcUaServer(
            serviceRegistration,
            [fromAssembly.Assembly],
            c => additionalConfiguration?.Invoke(c)
        );

    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistration serviceRegistration,
        IEnumerable<Assembly> assembliesContainingLoaders,
        AdditionalServerConfiguration? additionalConfiguration = null)
    {
        IServiceCollection collection = serviceRegistration.Collection;
        collection.AddSingleton(additionalConfiguration ?? (_ => { }));

        collection.AddSingleton(typeof(IEntityNodeStructureFactory<>), typeof(ReflectionBasedEntityStructureFactory<>));
        collection.AddServiceAndImplSingleton<IOpcUaTargetServerSetup, OpcUaTargetServerSetup>();

        var registration = typeof(ServiceExtensions)
            .CreateStaticMethodInvoker(nameof(AddServices), collection);

        foreach (var entity in OpcUaEntityTypes.Entities)
        {
            registration.Invoke(entity);
        }

        collection.AddServiceAndImplSingleton<IEntityNodeAccessConfigurator, NoAccessRestrictionsConfigurator>();
        collection.AddServiceAndImplSingleton<IServerStartedHealthCheck, ServerStartedHealthCheck>();
        collection.AddSingleton<IServerStartedHealthCheckMarker>(p => p.GetRequiredService<ServerStartedHealthCheck>());
        collection.AddServiceAndImplSingleton<IOpcUaEntityServerFactory, OpcUaEntityServerFactory>();
        collection.AddSingleton<OpcEntityServer>();
        collection.AddSingleton<IStartableEntityServer>(p =>
        {
            var factory = p.GetRequiredService<IOpcUaEntityServerFactory>();
            return factory.CreateServer();
        });
        AddLoaders(serviceRegistration.Collection, assembliesContainingLoaders);
        return new OnGoingOpcEntityServerServiceRegistration(serviceRegistration.Collection);
    }

    public static void AddServices<TEntity>(IServiceCollection collection)
    {
        collection
            .AddSingleton<IManagedEntityNodeSingletonFactory<TEntity>, ManagedEntityNodeSingletonFactory<TEntity>>();

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
        this OnGoingOpcEntityServiceRegistration serviceRegistration,
        Type assemblyMarker,
        Action<ServerConfiguration>? additionalConfiguration = null
    )
    {
        var serverConfig =
            serviceRegistration.WithOpcUaServer([assemblyMarker.Assembly], c => additionalConfiguration?.Invoke(c));
        serverConfig.Collection.AddHostedService<OpcUaServerBackgroundService>();
        return serverConfig;
    }

    private static void AddLoaders(IServiceCollection collection, IEnumerable<Assembly> assemblies)
    {
        Type loaderType = typeof(IEntityLoader<>);
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
            .Select(type =>
            {
                Type? foundLoaderInterface = type
                    .GetInterfaces()
                    .FirstOrDefault(@interface => @interface.Namespace == loaderType.Namespace
                                                  && @interface.IsConstructedGenericType &&
                                                  @interface.GetGenericTypeDefinition() == loaderType);

                if (foundLoaderInterface is null) return default;
                return (Service: foundLoaderInterface, Implementation: type);
            })
            .Where(result => result.Service is not null);

        foreach ((Type service, Type implementation) in loaders)
        {
            collection.AddServiceAndImplSingleton(service, implementation);
        }
    }
}