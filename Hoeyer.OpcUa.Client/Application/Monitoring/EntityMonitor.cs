

using System;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Core;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

[OpcUaEntityService(typeof(IEntityMonitor<>), ServiceLifetime.Singleton)]
public sealed class EntityMonitor<T>(
    IEntitySessionFactory factory,
    IEntityBrowser<T> browser,
    IReconnectionStrategy? reconnectionStrategy = null,
    EntityMonitoringConfiguration? entityMonitoringConfiguration = null) : IEntityMonitor<T>
{
    private readonly SubscriptionManager<T> _subscriptionManager = new();
    private ISession? Session { get; set; }
    private readonly EntityMonitoringConfiguration _entityMonitoringConfiguration = entityMonitoringConfiguration ?? new();
    private readonly IReconnectionStrategy _reconnectionStrategy = reconnectionStrategy ?? new DefaultReconnectStrategy();
    
    
    public async Task<IMessageSubscription> SubscribeToChange(
        IMessageConsumer<T> consumer,  CancellationToken cancellationToken = default)
    {
        Session ??= await factory.CreateSessionAsync("EntityMonitor");
        Session = await _reconnectionStrategy.ReconnectIfNotConnected(Session, cancellationToken);
        var node = await browser.BrowseEntityNode(Session, cancellationToken);
        
        var subscription = new Subscription(Session.DefaultSubscription)
        {
            PublishingInterval = _entityMonitoringConfiguration.WantedPublishingInterval.Milliseconds,
            SequentialPublishing = true,
            Priority = 0
        };

        var monitoredItem = new MonitoredItem
        {
            DisplayName = "MonitorNode",
            StartNodeId = node.Node.NodeId,
            AttributeId = Attributes.Value,
            SamplingInterval = 1000,
            QueueSize = 10,
            DiscardOldest = true
        };
        
        monitoredItem.Notification += HandleChange;

        subscription.AddItem(monitoredItem);
        Session.AddSubscription(subscription);
        await subscription.CreateAsync(cancellationToken);
        
        var s = _subscriptionManager.Subscribe(consumer);
        return new EventChangeSubscription(subscription, s);
    }

    private static void HandleChange(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        if (monitoredItem == null!) return;
        var notif  = e.NotificationValue as MonitoredItemNotification;
        if (notif == null) throw new ArgumentException("It was not a MonitoredItemNotification");
        throw new NotImplementedException();
    }

}