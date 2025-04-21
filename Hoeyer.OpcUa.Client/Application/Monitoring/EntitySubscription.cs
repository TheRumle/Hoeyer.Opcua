using System;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Core;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

[OpcUaEntityService(typeof(IEntitySubscriptionManager<>), ServiceLifetime.Singleton)]
public sealed class EntitySubscription<T>(IEntitySessionFactory sessionFactory, IMonitorItemsFactory<T> monitorFactory, IReconnectionStrategy? reconnectionStrategy = null) 
    : IEntitySubscriptionManager<T>
{
    private readonly SubscriptionManager<T> _subscriptionManager = new();
    public ISession? Session { get; private set; }
    public Subscription? Subscription { get; private set; }
    public MonitoredItem? ValueMonitor { get; private set; }
    
    private readonly IReconnectionStrategy _reconnectionStrategy = reconnectionStrategy ?? new DefaultReconnectStrategy();
    
    public async Task<IMessageSubscription> SubscribeToChange(
        IMessageConsumer<T> consumer,  CancellationToken cancellationToken = default)
    {
        Session ??= await sessionFactory.CreateSessionAsync("EntityMonitor");
        Session = await _reconnectionStrategy.ReconnectIfNotConnected(Session, cancellationToken);
        (Subscription, ValueMonitor) = await monitorFactory.GetOrCreate(Session, cancellationToken);
        return _subscriptionManager.Subscribe(consumer);
    }

    private static void HandleChange(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        if (monitoredItem == null!) return;
        var notif  = e.NotificationValue as MonitoredItemNotification;
        if (notif == null) throw new ArgumentException("It was not a MonitoredItemNotification");
        throw new NotImplementedException();
    }

}