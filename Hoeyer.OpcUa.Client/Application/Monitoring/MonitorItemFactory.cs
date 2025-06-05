using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

[OpcUaEntityService(typeof(IMonitorItemsFactory<>))]
public sealed class MonitorItemFactory<T>(
    ILogger<MonitorItemFactory<T>> logger,
    EntityMonitoringConfiguration entityMonitoringConfiguration) : IMonitorItemsFactory<T>
{
    private readonly Random _guids = new(231687);
    private bool _created = false;
    private Subscription Subscription { get; set; } = null!;
    private IEnumerable<MonitoredItem> MonitoredItems { get; set; } = [];


    public async ValueTask<(Subscription subscription, IEnumerable<MonitoredItem> variableMonitoring)> GetOrCreate(
        ISession session, IEntityNode node, CancellationToken cancel)
    {
        if (_created) return (Subscription, MonitoredItems);

        (Subscription, MonitoredItems) = Create(session, node);
        await Subscription.CreateAsync(cancel);
        await Subscription.ApplyChangesAsync(cancel);
        return (Subscription, MonitoredItems);
    }

    public (Subscription Subscription, List<MonitoredItem> items) Create(ISession session, IEntityNode node)
    {
        logger.LogInformation("Creating subscription and monitored items");

        var subscription = new Subscription(session!.DefaultSubscription)
        {
            PublishingInterval = entityMonitoringConfiguration!.ServerPublishingInterval.Milliseconds,
            SequentialPublishing = true,
            Priority = 0,
            DisplayName = node.BaseObject.BrowseName.Name + "Subscription",
            TransferId = _guids.GetUInt(),
        };

        var items = node
            .PropertyStates
            .Where(e => !MonitoredItems
                .Select(item => item.StartNodeId)
                .Contains(e.NodeId))
            .Select(e => CreateMonitoredItem(subscription, e.NodeId, e.BrowseName.Name))
            .Concat([CreateMonitoredItem(subscription, node.BaseObject.NodeId, node.BaseObject.BrowseName.Name)])
            .ToList();

        subscription.AddItems(items);
        if (session.Subscriptions.All(e => e.Id != subscription.Id))
        {
            session.AddSubscription(subscription);
        }

        _created = true;
        return (subscription, items);
    }

    private static MonitoredItem CreateMonitoredItem(Subscription subscription, NodeId nodeId, string name) =>
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