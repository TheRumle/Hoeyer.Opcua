using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Core;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

[OpcUaEntityService(typeof(IMonitorItemsFactory<>), ServiceLifetime.Singleton)]
public sealed class MonitorItemFactory<T>(IEntityBrowser<T> browser,  EntityMonitoringConfiguration? entityMonitoringConfiguration = null) : IMonitorItemsFactory<T>
{
    private readonly EntityMonitoringConfiguration _entityMonitoringConfiguration = entityMonitoringConfiguration ?? new();
    private Subscription? Subscription { get; set; }
    private MonitoredItem? MonitoredItem { get; set; }

    private static readonly uint TransferId = new Random().UInt();
    public async Task<(Subscription subscription, MonitoredItem variableMonitoring)> GetOrCreate(ISession session, CancellationToken cancellationToken = default)
    {
        if (Subscription != null && MonitoredItem != null) return (Subscription, MonitoredItem);
        return await Create(session, cancellationToken);
    }

    public async Task<(Subscription subscription, MonitoredItem variableMonitoring)> Create(ISession session, CancellationToken cancellationToken)
    {
        Subscription = new Subscription(session!.DefaultSubscription)
        {
            PublishingInterval = _entityMonitoringConfiguration.WantedPublishingInterval.Milliseconds,
            SequentialPublishing = true,
            Priority = 0,
            DisplayName = typeof(T).Name + "Subscription",
            TransferId = TransferId
        };

        var node = await browser.BrowseEntityNode(session, cancellationToken);
        MonitoredItem = new MonitoredItem(Subscription.DefaultItem)
        {
            DisplayName = typeof(T).Name+"Values",
            StartNodeId = node.Node.NodeId,
            AttributeId = Attributes.Value,
            SamplingInterval = 200,
            QueueSize = 10,
            DiscardOldest = true
        };
        Subscription.AddItem(MonitoredItem);
        
        if (session.Subscriptions.All(e => e.Id != Subscription.Id))
        {
            session.AddSubscription(Subscription);
        }

        await Subscription.CreateAsync(cancellationToken);
        
        return (Subscription, MonitoredItem);
    }
}