using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.Common.Messaging.Subscriptions.ChannelBased;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

[OpcUaEntityService(typeof(IEntitySubscriptionManager<>), ServiceLifetime.Singleton)]
internal sealed class EntitySubscriptionManager<T>(
    IEntitySessionFactory sessionFactory,
    IEntityBrowser<T> browser,
    IMonitorItemsFactory<T> monitorFactory,
    IEntityTranslator<T> translator,
    IReconnectionStrategy? reconnectionStrategy = null)
    : IEntitySubscriptionManager<T>
{
    private readonly IReconnectionStrategy _reconnectionStrategy =
        reconnectionStrategy ?? new DefaultReconnectStrategy();

    private readonly SubscriptionManager<T> _subscriptionManager = new(new ChannelSubscriptionFactory<T>());
    public ISession? Session { get; private set; }
    public IEnumerable<MonitoredItem> MonitoredItems { get; private set; } = [];
    private IEntityNode? CurrentNodeState { get; set; }
    public Subscription? Subscription { get; private set; }


    public async Task<IMessageSubscription> SubscribeToChange(
        IMessageConsumer<T> consumer, CancellationToken cancellationToken = default)
    {
        Session ??= await sessionFactory.CreateSessionAsync("EntityMonitor", cancellationToken);
        Session = await _reconnectionStrategy.ReconnectIfNotConnected(Session, cancellationToken);
        CurrentNodeState ??= await browser.BrowseEntityNode(cancellationToken);

        (Subscription, MonitoredItems) = monitorFactory.GetOrCreate(Session, CurrentNodeState);
        await Subscription.CreateAsync(cancellationToken);
        await Subscription.ApplyChangesAsync(cancellationToken);

        foreach (var items in MonitoredItems) items.Notification += HandleChange;

        await Session.PublishAsync(null, new SubscriptionAcknowledgementCollection(), cancellationToken);

        return _subscriptionManager.Subscribe(consumer);
    }

    private void HandleChange(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        if (monitoredItem == null!) return;

        Dictionary<string, PropertyState> properties = CurrentNodeState!.PropertyByBrowseName!;
        if (!properties.TryGetValue(monitoredItem.DisplayName, out PropertyState? property)) return;

        foreach (var newValue in monitoredItem.DequeueValues().Select(e => e.Value))
        {
            if (property.Value.Equals(newValue)) continue;
            property.Value = newValue;
            _subscriptionManager.Publish(translator.Translate(CurrentNodeState));
        }
    }
}