using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Client.Api.Calling;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.Application.Calling;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Hoeyer.OpcUa.Client.Application.Writing;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Client.Services;

public static class ClientServices
{
    public static OnGoingOpcEntityServiceRegistration WithOpcUaClientModelsFrom(
        this OnGoingOpcEntityServiceRegistration registration,
        Type fromAssembly) => WithOpcUaClientModelsFrom(registration, [fromAssembly]);

    public static OnGoingOpcEntityServiceRegistration WithOpcUaClientModelsFrom(
        this OnGoingOpcEntityServiceRegistration registration,
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
        services.AddServiceAndImplSingleton<BreadthFirstStrategy, BreadthFirstStrategy>();
        services.AddServiceAndImplSingleton<DepthFirstStrategy, DepthFirstStrategy>();
        services.AddServiceAndImplTransient(typeof(INodeTreeTraverser), conf.TraversalStrategy);
        services.AddServiceAndImplTransient(typeof(INodeReader), conf.NodeReader);
        services.AddServiceAndImplTransient(typeof(INodeBrowser), conf.Browser);
        services.AddServiceAndImplTransient(typeof(IMethodCaller<>), typeof(MethodCaller<>));
        services.AddServiceAndImplTransient(typeof(IEntityBrowser<>), typeof(EntityBrowser<>));
        services.AddServiceAndImplTransient(typeof(IMonitorItemsFactory<>), typeof(MonitorItemFactory<>));
        services.AddServiceAndImplTransient(typeof(IEntityWriter<>), typeof(EntityWriter<>));
        services.AddSingleton(typeof(EntityBehaviourImplementationModel<>));
        services.AddServiceAndImplSingleton<IEntitySessionFactory, ReusableSessionFactory>();
        services.AddServiceAndImplSingleton<ISubscriptionTransferStrategy, CopySubscriptionTransferStrategy>();
        services.AddServiceAndImplSingleton(typeof(IReconnectionStrategy), conf.ReconnectionStrategy);

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

        return services;
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