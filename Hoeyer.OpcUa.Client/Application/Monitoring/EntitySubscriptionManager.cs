using System;
using System.Collections;
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

    private readonly SubscriptionManager<T, ChannelBasedSubscription<T>> _subscriptionManager =
        new(new ChannelSubscriptionFactory<T>());

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

        (Subscription, MonitoredItems) = await monitorFactory.GetOrCreate(Session, CurrentNodeState, cancellationToken);
        foreach (var items in MonitoredItems) items.Notification += HandleChange;

        await Session.PublishAsync(null, new SubscriptionAcknowledgementCollection(), cancellationToken);
        return _subscriptionManager.Subscribe(consumer);
    }

    private void HandleChange(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        if (monitoredItem == null!) return;
        try
        {
            Dictionary<string, PropertyState> properties = CurrentNodeState!.PropertyByBrowseName!;
            if (!properties.TryGetValue(monitoredItem.DisplayName, out PropertyState? property)) return;

            var currentValue = property.WrappedValue.Value;
            IEnumerable<object> newValues = monitoredItem.DequeueValues().Select(e => e.Value);

            foreach (var newValue in newValues.Where(newVal => !IsSameValue(currentValue, newVal)))
            {
                property.Value = newValue;
                _subscriptionManager.Publish(translator.Translate(CurrentNodeState));
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private static bool IsSameValue(object currentValue, object newValue)
    {
        if (currentValue.Equals(newValue)) return true;

        if (currentValue is IEnumerable currentEnumerable &&
            newValue is IEnumerable newEnumerable)
        {
            HashSet<object> currentSet = currentEnumerable.Cast<object>().ToHashSet();
            HashSet<object> newSet = newEnumerable.Cast<object>().ToHashSet();

            if (currentSet.SetEquals(newSet)) return true;
        }

        return false;
    }
}