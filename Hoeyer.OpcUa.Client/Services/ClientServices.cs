using System;
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
using Hoeyer.OpcUa.Client.Application.Browsing.Reading;
using Hoeyer.OpcUa.Client.Application.Calling;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Hoeyer.OpcUa.Client.Application.Writing;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Client.Services;

public static class ClientServices
{
    public static OnGoingOpcEntityServiceRegistration WithOpcUaClientServices(
        this OnGoingOpcEntityServiceRegistration registration, Action<ClientServiceConfiguration>? configure = null)
    {
        var conf = new ClientServiceConfiguration();
        configure?.Invoke(conf);

        IServiceCollection services = registration.Collection;
        foreach (var (service, impl, _) in OpcUaEntityTypes.EntityBehaviours)
        {
            services.AddServiceAndImplSingleton(service, impl);
        }

        services.AddLogging();
        services.AddServiceAndImplTransient<BreadthFirstStrategy, BreadthFirstStrategy>();
        services.AddServiceAndImplTransient<DepthFirstStrategy, DepthFirstStrategy>();
        services.AddServiceAndImplSingleton(typeof(INodeTreeTraverser), conf.TraversalStrategy);
        services.AddServiceAndImplSingleton(typeof(INodeReader), conf.NodeReader);
        services.AddServiceAndImplTransient(typeof(INodeBrowser), conf.Browser);
        services.AddServiceAndImplSingleton<IEntitySessionFactory, ReusableSessionFactory>();
        services.AddServiceAndImplTransient<ISubscriptionTransferStrategy, CopySubscriptionTransferStrategy>();
        services.AddServiceAndImplTransient(typeof(IReconnectionStrategy), conf.ReconnectionStrategy);
        services.AddSingleton<EntityMonitoringConfiguration>();


        var builder = typeof(ClientServices).InvokeStaticEntityRegistration(nameof(RegisterEntityGenerics), services);
        foreach (var entity in OpcUaEntityTypes.Entities)
        {
            builder.Invoke(entity);
        }

        return registration;
    }

    private static void RegisterEntityGenerics<TEntity>(IServiceCollection services)
    {
        services.AddServiceAndImplTransient<IEntityBrowser<TEntity>, EntityBrowser<TEntity>>();
        services.AddServiceAndImplTransient<IMonitorItemsFactory<TEntity>, MonitorItemFactory<TEntity>>();
        services.AddServiceAndImplTransient<IEntityWriter<TEntity>, EntityWriter<TEntity>>();
        services.AddServiceAndImplSingleton<IEntitySubscriptionManager<TEntity>, EntitySubscriptionManager<TEntity>>();
        services.AddServiceAndImplTransient<ICurrentEntityStateChannel<TEntity>, CurrentEntityStateChannel<TEntity>>();
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
        services.AddServiceAndImplTransient(typeof(IMethodCaller<>), typeof(MethodCaller<>));
    }
}

public sealed class ClientServiceConfiguration
{
    public Type TraversalStrategy { get; private set; } = typeof(BreadthFirstStrategy);
    public Type Browser { get; private set; } = typeof(NodeBrowser);
    public Type NodeReader { get; private set; } = typeof(NodeReader);
    public Type ReconnectionStrategy { get; private set; } = typeof(DefaultReconnectStrategy);
    public EntityMonitoringConfiguration EntityMonitoringConfiguration { get; } = new();

    public ClientServiceConfiguration WithNodeTreeTraversalStrategy<TStrategy>() where TStrategy : INodeTreeTraverser
    {
        TraversalStrategy = typeof(TStrategy);
        return this;
    }

    public ClientServiceConfiguration WithNodeBrowser<TBrowser>() where TBrowser : INodeBrowser
    {
        Browser = typeof(TBrowser);
        return this;
    }

    public ClientServiceConfiguration WithNodeReader<TReader>() where TReader : INodeReader
    {
        NodeReader = typeof(TReader);
        return this;
    }

    public ClientServiceConfiguration WithReconnectionStrategy<TStrategy>() where TStrategy : IReconnectionStrategy
    {
        ReconnectionStrategy = typeof(IReconnectionStrategy);
        return this;
    }


    public ClientServiceConfiguration WithMonitoringConfiguration(Action<EntityMonitoringConfiguration> configuration)
    {
        configuration.Invoke(EntityMonitoringConfiguration);
        return this;
    }
}