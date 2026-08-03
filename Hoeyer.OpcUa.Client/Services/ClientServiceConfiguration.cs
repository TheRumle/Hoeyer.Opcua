using System;
using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Browsing.Reading;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.Application.Browsing.Reading;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Opc.Ua;
using INodeBrowser = Hoeyer.OpcUa.Client.Abstractions.Browsing.INodeBrowser;
using NodeBrowser = Hoeyer.OpcUa.Client.Application.Browsing.NodeBrowser;

namespace Hoeyer.OpcUa.Client.Services;

public sealed class ClientServiceConfiguration
{
    public static readonly ClientServiceConfiguration Default = new();
    public Action<ApplicationConfiguration> clientConfig { get; private set; } = (_) => { };
    public Type TraversalStrategy { get; private set; } = typeof(BreadthFirstStrategy);
    public Type Browser { get; private set; } = typeof(NodeBrowser);
    public Type NodeReader { get; private set; } = typeof(NodeReader);
    public Type ReconnectionStrategy { get; private set; } = typeof(DefaultReconnectStrategy);
    public Type EntitySessionFactory { get; private set; } = typeof(CachedSessionFactory);
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
        ReconnectionStrategy = typeof(TStrategy);
        return this;
    }


    public ClientServiceConfiguration WithEntitySessionFactory<TFactory>() where TFactory : IEntitySessionFactory
    {
        EntitySessionFactory = typeof(TFactory);
        return this;
    }

    public ClientServiceConfiguration ConfigureClientApplication(Action<ApplicationConfiguration> appConfiguration)
    {
        this.clientConfig = appConfiguration;
        return this;
    }


    public ClientServiceConfiguration WithMonitoringConfiguration(Action<EntityMonitoringConfiguration> configuration)
    {
        configuration.Invoke(EntityMonitoringConfiguration);
        return this;
    }
}