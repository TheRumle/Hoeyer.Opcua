using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

[OpcUaEntityService(typeof(IMonitorItemsFactory<>))]
public sealed class MonitorItemFactory<T>(ILogger<MonitorItemFactory<T>> logger,
    EntityMonitoringConfiguration entityMonitoringConfiguration) : IMonitorItemsFactory<T>
{
    private Subscription? Subscription { get; set; }
    private IEnumerable<MonitoredItem> MonitoredItems { get; set; } = [];
    private readonly Random _guids = new Random(231687);

    
    public (Subscription subscription, IEnumerable<MonitoredItem> variableMonitoring)
        GetOrCreate(ISession session, IEntityNode node)
    {
        Subscription ??= new Subscription(session!.DefaultSubscription)
        {
            PublishingInterval = entityMonitoringConfiguration!.ServerPublishingInterval.Milliseconds,
            SequentialPublishing = true,
            Priority = 0,
            DisplayName = node.BaseObject.BrowseName.Name + "Subscription",
            TransferId = _guids.GetUInt(),
        };
        return Create(session, node);
    }

    public (Subscription Subscription, List<MonitoredItem> items) Create(ISession session, IEntityNode node)
    {
        logger.LogInformation("Creating subscription and monitored items");
        var items = node
            .PropertyStates
            .Where(e => !MonitoredItems
                .Select(item => item.StartNodeId)
                .Contains(e.NodeId))
            .Select(e => CreateMonitoredItem(e.NodeId, e.BrowseName.Name))
            .Concat([CreateMonitoredItem(node.BaseObject.NodeId, node.BaseObject.BrowseName.Name)])
            .ToList();
        
        Subscription!.AddItems(items);
        
        if (session.Subscriptions.All(e => e.Id != Subscription.Id))
        {
            session.AddSubscription(Subscription);
        }
        
        return (Subscription, items);
    }

    private MonitoredItem CreateMonitoredItem(NodeId nodeId, string name)
    {
        return new MonitoredItem(Subscription!.DefaultItem)
        {
            DisplayName = name,
            StartNodeId = nodeId,
            AttributeId = Attributes.Value,
            SamplingInterval = 200,
            QueueSize = 10,
            DiscardOldest = true
        };
    }
}