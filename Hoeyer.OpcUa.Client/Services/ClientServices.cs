using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Browsing.Reading;
using Hoeyer.OpcUa.Client.Abstractions.Calling;
using Hoeyer.OpcUa.Client.Abstractions.Configuration;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Client.Abstractions.Monitoring;
using Hoeyer.OpcUa.Client.Abstractions.Writing;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.Application.Calling;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Hoeyer.OpcUa.Client.Application.Writing;
using Hoeyer.OpcUa.Client.Configuration;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua.Client;
using INodeBrowser = Hoeyer.OpcUa.Client.Abstractions.Browsing.INodeBrowser;

namespace Hoeyer.OpcUa.Client.Services;

public static class ClientServices
{
    public static OnGoingOpcEntityServiceRegistrationWithModels WithOpcUaClientModelsFrom(
        this OnGoingOpcEntityServiceRegistrationWithModels registration,
        Type fromAssembly) => WithOpcUaClientModelsFrom(registration, [fromAssembly]);

    public static OnGoingOpcEntityServiceRegistrationWithModels WithOpcUaClientModelsFrom(
        this OnGoingOpcEntityServiceRegistrationWithModels registration,
        IEnumerable<Type> fromAssembly,
        Action<ClientServiceConfiguration>? configure = null
    )
    {
        var conf = new ClientServiceConfiguration();
        configure?.Invoke(conf);

        registration.Collection.AddClientServices(fromAssembly, conf);
        return registration;
    }

    public static IServiceCollection AddClientServices(
        this IServiceCollection services,
        IEnumerable<Type> fromAssembly,
        ClientServiceConfiguration conf)
    {
        var markers = fromAssembly.ToList();
        services.AddSingleton(conf.EntityMonitoringConfiguration);
        services.AddKeyedSingleton(ServiceKeys.CLIENT_SERVICES, markers.Select(e => new AssemblyMarker(e)));

        services.AddSingleton(new ClientApplicationConfigurationAction(conf.clientConfig));
        services.AddSingleton<IClientApplicationConfigurationFactory, ClientApplicationConfigurationFactory>();


        services.AddNodeBrowsing(conf);
        services.AddSessionManagement(conf);
        services.AddMethodCalling();


        services.AddServiceAndImplTransient(typeof(IEntityWriter<>), typeof(EntityWriter<>));


        return services;
    }

    private static void AddMethodCalling(this IServiceCollection services)
    {
        services.AddServiceAndImplTransient(typeof(IMethodCaller<>), typeof(MethodCaller<>));
        services.AddSingleton(typeof(EntityBehaviourImplementationModel<>));
        var provider = services.BuildServiceProvider();
        var entities = provider.GetRequiredService<EntityTypesCollection>().ModelledEntities;
        var subscriptionEngineRegistration =
            typeof(ClientServices).CreateStaticMethodInvoker(nameof(RegisterSubscriptionEngine), services);
        var behaviourImplementationRegistration =
            typeof(ClientServices).CreateStaticMethodInvoker(nameof(RegisterEntityBehaviour), services, provider);
        foreach (var entity in entities)
        {
            subscriptionEngineRegistration.Invoke(entity);
            behaviourImplementationRegistration.Invoke(entity);
        }
    }

    private static void AddSessionManagement(this IServiceCollection services, ClientServiceConfiguration conf)
    {
        services.AddServiceAndImplTransient(typeof(IMonitorItemFactory<>), typeof(MonitorItemFactory<>));
        services.AddSingleton<ISessionFactory, DefaultSessionFactory>();
        services.AddServiceAndImplSingleton(typeof(IEntitySessionFactory), conf.EntitySessionFactory);
        services.AddSingleton<EntitySessionFactory>();
        services.AddServiceAndImplSingleton<ISubscriptionTransferStrategy, CopySubscriptionTransferStrategy>();
        services.AddServiceAndImplSingleton(typeof(IReconnectionStrategy), conf.ReconnectionStrategy);
    }

    private static void AddNodeBrowsing(this IServiceCollection services, ClientServiceConfiguration conf)
    {
        services.AddServiceAndImplTransient<INodeTreeTraverser, BreadthFirstStrategy>();
        services.AddServiceAndImplTransient<INodeTreeTraverser, DepthFirstStrategy>(); //default
        services.AddServiceAndImplTransient(typeof(INodeTreeTraverser), conf.TraversalStrategy);
        services.AddServiceAndImplTransient(typeof(INodeReader), conf.NodeReader);
        services.AddServiceAndImplTransient(typeof(INodeBrowser), conf.Browser);
        services.AddServiceAndImplTransient(typeof(IEntityBrowser<>), typeof(EntityBrowser<>));
    }

    private static void RegisterEntityBehaviour<TEntity>(IServiceCollection services, IServiceProvider provider)
    {
        var behaviourModel = provider.GetRequiredService<EntityBehaviourImplementationModel<TEntity>>();
        foreach (var (service, impl) in behaviourModel.MethodImplementors)
        {
            services.AddSingleton(service, impl);
        }
    }

    private static void RegisterSubscriptionEngine<TEntity>(IServiceCollection services)
    {
        services.AddServiceAndImplSingleton(typeof(IEntitySubscriptionManager<TEntity>),
            typeof(EntitySubscriptionManager<TEntity>));
        services.AddServiceAndImplTransient(typeof(ICurrentEntityStateChannel<TEntity>),
            typeof(CurrentEntityStateChannel<TEntity>));
        services.AddTransient<IStateChangeObserver<TEntity>>(provider =>
        {
            return new StateChangeObserver<TEntity>(
                new Lazy<(Task<IMessageSubscription> subscriptionTask, ICurrentEntityStateChannel<TEntity>
                    currentEntityStateChannel)>(() =>
                {
                    var currentEntityStateChannel = provider.GetRequiredService<ICurrentEntityStateChannel<TEntity>>();
                    var subscriptionManager = provider.GetRequiredService<IEntitySubscriptionManager<TEntity>>();
                    var subscription = subscriptionManager.SubscribeToAllPropertyChanges(currentEntityStateChannel);
                    return (subscription, currentEntityStateChannel);
                }));
        });
    }
}