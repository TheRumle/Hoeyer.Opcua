﻿using System;
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
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Subscriptions;

[OpcUaEntityService(typeof(IEntitySubscriptionManager<>), ServiceLifetime.Singleton)]
internal sealed class EntitySubscriptionManager<T>(
    ILogger<EntitySubscriptionManager<T>> logger,
    IEntitySessionFactory sessionFactory,
    IEntityBrowser<T> browser,
    IMonitorItemsFactory<T> monitorFactory,
    IEntityTranslator<T> translator)
    : IEntitySubscriptionManager<T>
{
    private static readonly string SessionClientId = typeof(T).Name + "EntityMonitor";

    private readonly SemaphoreSlim _lock = new(1);

    private readonly SubscriptionManager<T, ChannelBasedSubscription<T>> _subscriptionManager =
        new(new ChannelSubscriptionFactory<T>());

    private List<MonitoredItem> MonitoredItems { get; set; } = [];
    private IEntityNode? CurrentNodeState { get; set; }
    public Subscription? Subscription { get; private set; }


    public async Task<IMessageSubscription> SubscribeToChange(
        IMessageConsumer<T> consumer, CancellationToken cancellationToken = default)
    {
        await BeginMonitorNodes(cancellationToken);
        return _subscriptionManager.Subscribe(consumer);
    }

    private async Task BeginMonitorNodes(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (MonitoredItems.Count > 0)
            {
                return;
            }

            CurrentNodeState ??= await browser.BrowseEntityNode(cancellationToken);
            var session = await sessionFactory.GetSessionForAsync(SessionClientId, cancellationToken);
            var (subscription, items) = await monitorFactory.GetOrCreate(session, CurrentNodeState, cancellationToken);
            (Subscription, MonitoredItems) = (subscription, items.ToList());

            foreach (var item in MonitoredItems)
            {
                item.Notification += HandleChange;
            }

            await session.Session.PublishAsync(null, new SubscriptionAcknowledgementCollection(), cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    private void HandleChange(MonitoredItem item, MonitoredItemNotificationEventArgs eventArgs)
    {
        if (item == null!)
        {
            return;
        }

        try
        {
            var properties = CurrentNodeState!.PropertyByBrowseName!;
            if (!properties.TryGetValue(item.DisplayName, out var property))
            {
                logger.LogInformation("Entity does not have any property named {Item}", item.DisplayName);
                return;
            }

            var values = item.DequeueValues().ToArray();
            foreach (var value in values)
            {
                property.Value = value.Value;
                _subscriptionManager.Publish(translator.Translate(CurrentNodeState));
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error while processing change notification for {Item}", item.DisplayName);
        }
    }
}