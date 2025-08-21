using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructureFactory;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Services;
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
        Action<ServerConfiguration>? additionalConfiguration = null)
    {
        IServiceCollection collection = serviceRegistration.Collection;

        collection.AddSingleton(typeof(IEntityNodeStructureFactory<>), typeof(ReflectionBasedEntityStructureFactory<>));
        collection.AddSingleton<OpcUaEntityServerSetup>(p =>
        {
            var standardConfig = p.GetService<IOpcUaEntityServerInfo>();
            if (standardConfig == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(IOpcUaEntityServerInfo)} has been registered! This should be prevented using builder pattern. Are you using the library as intended and using the {nameof(ServiceCollectionExtensions.AddOpcUaServerConfiguration)} {nameof(IServiceCollection)} extension method?");
            }

            return new OpcUaEntityServerSetup(standardConfig, additionalConfiguration ?? (value => { }));
        });

        var registration = typeof(ServiceExtensions).InvokeStaticEntityRegistration(nameof(AddServices), collection);
        foreach (var entity in OpcUaEntityTypes.Entities)
        {
            registration.Invoke(entity);
        }

        collection.AddServiceAndImplSingleton<IEntityNodeAccessConfigurator, NoAccessRestrictionsConfigurator>();
        collection.AddSingleton<EntityServerStartedMarker>();
        collection.AddSingleton<OpcUaEntityServerFactory>();
        collection.AddSingleton<OpcEntityServer>();
        collection.AddSingleton<IStartableEntityServer>(p =>
        {
            var factory = p.GetRequiredService<OpcUaEntityServerFactory>();
            return factory.CreateServer();
        });
        AddLoaders(serviceRegistration.Collection);


        return new OnGoingOpcEntityServerServiceRegistration(serviceRegistration.Collection);
    }

    public static void AddServices<TEntity>(IServiceCollection collection)
    {
        collection.AddServiceAndImplSingleton<IManagedEntityNodeSingletonFactory<TEntity>>(
            typeof(ManagedEntityNodeSingletonFactory<TEntity>));
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
        Action<ServerConfiguration>? additionalConfiguration = null)
    {
        var serverConfig = serviceRegistration.WithOpcUaServer(additionalConfiguration);
        serverConfig.Collection.AddHostedService<OpcUaServerBackgroundService>();
        return serverConfig;
    }

    public static OpcUaEntityServerSetup WithAdditionalServerConfiguration(IOpcUaEntityServerInfo setup,
        Action<ServerConfiguration> additionalConfiguration)
    {
        return new OpcUaEntityServerSetup(setup, additionalConfiguration);
    }

    private static void AddLoaders(IServiceCollection collection)
    {
        Type loaderType = typeof(IEntityLoader<>);
        IEnumerable<(Type Service, Type Implementation)> loaders = OpcUaEntityTypes
            .TypesFromReferencingAssemblies
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