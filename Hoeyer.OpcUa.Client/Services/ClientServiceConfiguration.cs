using System;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.Application.Browsing.Reading;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.Application.Subscriptions;

namespace Hoeyer.OpcUa.Client.Services;

public sealed class ClientServiceConfiguration
{
    public static readonly ClientServiceConfiguration Default = new();
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