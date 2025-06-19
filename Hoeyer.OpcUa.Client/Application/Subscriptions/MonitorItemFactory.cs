using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Connection;
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
    private readonly Random _guids = new(231687);
    private bool _created;
    private EntitySubscription Subscription { get; set; } = null!;
    private IEnumerable<MonitoredItem> MonitoredItems { get; set; } = [];


    public async ValueTask<(EntitySubscription subscription, IEnumerable<MonitoredItem> variableMonitoring)>
        GetOrCreate(IEntitySession session, IEntityNode node, CancellationToken cancel)
    {
        if (_created)
        {
            return (Subscription, MonitoredItems);
        }

        (Subscription, MonitoredItems) = Create(session, node);
        await Subscription.CreateAsync(cancel);
        await Subscription.ApplyChangesAsync(cancel);
        return (Subscription, MonitoredItems);
    }

    public (EntitySubscription Subscription, List<MonitoredEntityItem> items) Create(IEntitySession session,
        IEntityNode node)
    {
        logger.LogInformation("Creating subscription and monitored items");

        var subscription = new EntitySubscription(session)
        {
            PublishingInterval = entityMonitoringConfiguration!.ServerPublishingInterval.Milliseconds,
            SequentialPublishing = true,
            Priority = 0,
            DisplayName = node.BaseObject.BrowseName.Name + "Subscription",
            TransferId = _guids.GetUInt()
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
        if (session.EntitySubscriptions.All(e => e.Id != subscription.Id))
        {
            session.Session.AddSubscription(subscription);
        }

        _created = true;
        return (subscription, items);
    }

    private static MonitoredEntityItem CreateMonitoredItem(EntitySubscription subscription, NodeId nodeId,
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