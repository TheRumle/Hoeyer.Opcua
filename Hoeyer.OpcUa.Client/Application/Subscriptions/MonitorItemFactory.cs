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

[OpcUaEntityService(typeof(IMonitorItemsFactory<>))]
public sealed class MonitorItemFactory<T>(
    ILogger<MonitorItemFactory<T>> logger,
    EntityMonitoringConfiguration entityMonitoringConfiguration) : IMonitorItemsFactory<T>
{
    private static readonly int NumberOfProperties = typeof(T).GetProperties().Length;
    private readonly Random _guids = new(231687);
    private bool _subscriptionCreated;
    private bool AllItemsMonitored => MonitoredItemsByName.Values.Count == NumberOfProperties;

    private EntitySubscription Subscription { get; set; } = null!;
    private Dictionary<string, MonitoredEntityItem> MonitoredItemsByName { get; } = [];
    private IEnumerable<MonitoredEntityItem> MonitoredItems => MonitoredItemsByName.Values;


    public async ValueTask<(EntitySubscription subscription, IReadOnlyList<MonitoredItem> variableMonitoring)>
        CreateAndMonitorAll(IEntitySession session,
            IEntityNode node,
            Action<MonitoredItem, MonitoredItemNotificationEventArgs> callback,
            CancellationToken cancel = default)
    {
        await GetOrCreateSubscriptionWithCallback(session, "Name", callback, cancel);
        await MonitorAllProperties(Subscription, node, cancel);
        return (Subscription, MonitoredItems.ToList());
    }


    public async ValueTask<IReadOnlyList<MonitoredEntityItem>> MonitorAllProperties(EntitySubscription subscription,
        IEntityNode node, CancellationToken cancel = default)
    {
        IEnumerable<(NodeId Id, string Name)> nodes =
        [
            (node.BaseObject.NodeId, node.BaseObject.BrowseName.Name),
            ..node.PropertyStates.Select(node => (node.NodeId, node.BrowseName.Name))
        ];

        return await MonitorProperties(subscription, nodes, cancel);
    }

    public async ValueTask<IReadOnlyList<MonitoredEntityItem>> MonitorProperties(
        EntitySubscription subscription,
        IEnumerable<(NodeId Id, string Name)> nodesToMonitor,
        CancellationToken cancel = default)
    {
        var nodes = nodesToMonitor.ToList();
        if (AllItemsMonitored)
        {
            return Subscription.EntityItems.Where(monitored => nodes.Select(e => e.Id).Contains(monitored.StartNodeId))
                .ToList();
        }

        using var scope = logger.BeginScope("Creating monitored items");
        var items = nodes
            .Where(e => !MonitoredItems
                .Select(item => item.StartNodeId)
                .Contains(e.Id))
            .Select(e => CreateMonitoredItem(subscription, e.Id, e.Name))
            .ToList();

        foreach (var monitoredEntityItem in items.Where(item =>
                     !MonitoredItemsByName.ContainsKey(item.DisplayName)))
        {
            MonitoredItemsByName[monitoredEntityItem.DisplayName] = monitoredEntityItem;
            subscription.AddEntityItem(monitoredEntityItem);
        }

        await Subscription.ApplyChangesAsync(cancel);
        return items;
    }

    public async ValueTask<MonitoredEntityItem> MonitorProperty(
        EntitySubscription subscription,
        (NodeId Id, string Name) nodeToMonitor,
        CancellationToken cancel = default) =>
        (await MonitorProperties(subscription, [nodeToMonitor], cancel)).First();

    public async ValueTask<EntitySubscription> GetOrCreateSubscriptionWithCallback(
        IEntitySession session,
        string subscriptionName,
        Action<MonitoredItem, MonitoredItemNotificationEventArgs> callback,
        CancellationToken cancel = default)
    {
        if (_subscriptionCreated)
        {
            return Subscription;
        }

        using var scope = logger.BeginScope("Creating subscription");
        Subscription = new EntitySubscription(session, callback)
        {
            PublishingInterval = entityMonitoringConfiguration!.ServerPublishingInterval.Milliseconds,
            SequentialPublishing = true,
            Priority = 0,
            DisplayName = subscriptionName,
            TransferId = _guids.GetUInt()
        };
        if (session.EntitySubscriptions.All(e => e.Id != Subscription.Id))
        {
            session.Session.AddSubscription(Subscription);
        }

        await Subscription.CreateAsync(cancel);
        _subscriptionCreated = true;
        return Subscription;
    }

    private static MonitoredEntityItem CreateMonitoredItem(
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