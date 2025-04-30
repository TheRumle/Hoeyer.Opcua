using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.Extensions;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Entity.Node;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

[OpcUaEntityService(typeof(IEntitySubscriptionManager<>), ServiceLifetime.Singleton)]
internal sealed class EntitySubscriptionManager<T>(
    IEntitySessionFactory sessionFactory,
    IEntityBrowser<T> browser,
    IMonitorItemsFactory<T> monitorFactory,
    ILogger<EntitySubscriptionManager<T>> logger, 
    IReconnectionStrategy? reconnectionStrategy = null)
    : IEntitySubscriptionManager<T>
{
    private readonly SubscriptionManager<T> _subscriptionManager = new();
    public ISession? Session { get; private set; }
    public Subscription? Subscription { get; private set; }
    public IEnumerable<MonitoredItem>? MonitoredItems { get; private set; }
    
    private readonly IReconnectionStrategy _reconnectionStrategy = reconnectionStrategy ?? new DefaultReconnectStrategy();
    
    public async Task<IMessageSubscription> SubscribeToChange(
        IMessageConsumer<T> consumer,  CancellationToken cancellationToken = default)
    {
        Session ??= await sessionFactory.CreateSessionAsync("EntityMonitor");
        Session = await _reconnectionStrategy.ReconnectIfNotConnected(Session, cancellationToken);
        IEntityNode node = browser.LastState?.node ?? await browser.BrowseEntityNode(cancellationToken);
        
        (Subscription, MonitoredItems) = monitorFactory.GetOrCreate(Session, node);
        await Subscription.CreateAsync(cancellationToken);
        await Subscription.ApplyChangesAsync(cancellationToken);

        foreach (var items in MonitoredItems) items.Notification += HandleChange;
        
        await Session.PublishAsync(null, new SubscriptionAcknowledgementCollection(), cancellationToken);
        
        return _subscriptionManager.Subscribe(consumer);
    }

    private void HandleChange(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        if (monitoredItem == null!) return;
        _subscriptionManager.Publish(default!);
    }

}