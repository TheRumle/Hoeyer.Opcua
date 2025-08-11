using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Subscriptions;

[OpcUaAgentService(typeof(IMonitorItemsFactory<>))]
public sealed class MonitorItemFactory<T>(
    ILogger<MonitorItemFactory<T>> logger,
    AgentMonitoringConfiguration agentMonitoringConfiguration) : IMonitorItemsFactory<T>
{
    private static readonly int NumberOfProperties = typeof(T).GetProperties().Length;
    private readonly Random _guids = new(231687);
    private bool _subscriptionCreated;
    private bool AllItemsMonitored => MonitoredItemsByName.Values.Count == NumberOfProperties;

    private AgentSubscription Subscription { get; set; } = null!;
    private Dictionary<string, MonitoredAgentItem> MonitoredItemsByName { get; } = [];
    private IEnumerable<MonitoredAgentItem> MonitoredItems => MonitoredItemsByName.Values;


    public async ValueTask<(AgentSubscription subscription, IReadOnlyList<MonitoredItem> variableMonitoring)>
        CreateAndMonitorAll(IAgentSession session,
            IAgent node,
            Action<MonitoredItem, MonitoredItemNotificationEventArgs> callback,
            CancellationToken cancel = default)
    {
        await GetOrCreateSubscriptionWithCallback(session, "Name", callback, cancel);
        await MonitorAllProperties(Subscription, node, cancel);
        return (Subscription, MonitoredItems.ToList());
    }


    public async ValueTask<IReadOnlyList<MonitoredAgentItem>> MonitorAllProperties(AgentSubscription subscription,
        IAgent node, CancellationToken cancel = default)
    {
        IEnumerable<(NodeId Id, string Name)> nodes =
        [
            (node.BaseObject.NodeId, node.BaseObject.BrowseName.Name),
            ..node.PropertyStates.Select(node => (node.NodeId, node.BrowseName.Name))
        ];

        return await MonitorProperties(subscription, nodes, cancel);
    }

    public async ValueTask<IReadOnlyList<MonitoredAgentItem>> MonitorProperties(
        AgentSubscription subscription,
        IEnumerable<(NodeId Id, string Name)> nodesToMonitor,
        CancellationToken cancel = default)
    {
        var nodes = nodesToMonitor.ToList();
        if (AllItemsMonitored)
        {
            return Subscription.AgentItems.Where(monitored => nodes.Select(e => e.Id).Contains(monitored.StartNodeId))
                .ToList();
        }

        using var scope = logger.BeginScope("Creating monitored items");
        var items = nodes
            .Where(e => !MonitoredItems
                .Select(item => item.StartNodeId)
                .Contains(e.Id))
            .Select(e => CreateMonitoredItem(subscription, e.Id, e.Name))
            .ToList();

        foreach (var monitoredAgentItem in items.Where(item =>
                     !MonitoredItemsByName.ContainsKey(item.DisplayName)))
        {
            MonitoredItemsByName[monitoredAgentItem.DisplayName] = monitoredAgentItem;
            subscription.AddAgentItem(monitoredAgentItem);
        }

        await Subscription.ApplyChangesAsync(cancel);
        return items;
    }

    public async ValueTask<MonitoredAgentItem> MonitorProperty(
        AgentSubscription subscription,
        (NodeId Id, string Name) nodeToMonitor,
        CancellationToken cancel = default) =>
        (await MonitorProperties(subscription, [nodeToMonitor], cancel)).First();

    public async ValueTask<AgentSubscription> GetOrCreateSubscriptionWithCallback(
        IAgentSession session,
        string subscriptionName,
        Action<MonitoredItem, MonitoredItemNotificationEventArgs> callback,
        CancellationToken cancel = default)
    {
        if (_subscriptionCreated)
        {
            return Subscription;
        }

        using var scope = logger.BeginScope("Creating subscription");
        Subscription = new AgentSubscription(session, callback)
        {
            PublishingInterval = agentMonitoringConfiguration!.ServerPublishingInterval.Milliseconds,
            SequentialPublishing = true,
            Priority = 0,
            DisplayName = subscriptionName,
            TransferId = _guids.GetUInt()
        };
        if (session.AgentSubscriptions.All(e => e.Id != Subscription.Id))
        {
            session.Session.AddSubscription(Subscription);
        }

        await Subscription.CreateAsync(cancel);
        _subscriptionCreated = true;
        return Subscription;
    }

    private static MonitoredAgentItem CreateMonitoredItem(
        Subscription subscription, NodeId nodeId,
        string name) =>
        new(subscription!.DefaultItem)
        {
            DisplayName = name,
            StartNodeId = nodeId,
            AttributeId = Attributes.Value,
            SamplingInterval = 200,
            QueueSize = 10,
            DiscardOldest = true
        };
}