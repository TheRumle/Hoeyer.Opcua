using System;
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
using Hoeyer.OpcUa.Client.Extensions;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Subscriptions;

[OpcUaAgentService(typeof(IAgentSubscriptionManager<>), ServiceLifetime.Singleton)]
internal sealed class AgentSubscriptionManager<T>(
    ILogger<AgentSubscriptionManager<T>> logger,
    IAgentSessionFactory sessionFactory,
    IAgentBrowser<T> browser,
    IMonitorItemsFactory<T> monitorFactory,
    IAgentTranslator<T> translator)
    : IAgentSubscriptionManager<T>
{
    private static readonly string SessionClientId = typeof(T).Name + "AgentMonitor";

    private readonly SubscriptionManager<T, ChannelBasedSubscription<T>> _subscriptionManager =
        new(new ChannelSubscriptionFactory<T>());

    private IReadOnlyList<MonitoredItem> MonitoredItems { get; set; } = [];
    private IAgent? CurrentNodeState { get; set; }
    private AgentSubscription? AgentSubscription { get; set; }
    public Subscription? Subscription => AgentSubscription;


    public async Task<IMessageSubscription> SubscribeToAllPropertyChanges(
        IMessageConsumer<T> consumer, CancellationToken cancellationToken = default)
    {
        CurrentNodeState ??= await browser.BrowseAgent(cancellationToken);
        var session = await sessionFactory.GetSessionForAsync(SessionClientId, cancellationToken);
        (AgentSubscription, MonitoredItems) =
            await monitorFactory.CreateAndMonitorAll(session, CurrentNodeState, HandleChange, cancellationToken);
        await session.Session.PublishAsync(null, new SubscriptionAcknowledgementCollection(), cancellationToken);
        return _subscriptionManager.Subscribe(consumer);
    }

    /// <inheritdoc />
    public async Task<IMessageSubscription> SubscribeToProperty(
        IMessageConsumer<T> consumer,
        string propertyName,
        CancellationToken cancellationToken = default)
    {
        CurrentNodeState ??= await browser.BrowseAgent(cancellationToken);
        var session = await sessionFactory.GetSessionForAsync(SessionClientId, cancellationToken);
        AgentSubscription ??= await monitorFactory.GetOrCreateSubscriptionWithCallback(
            session,
            "AgentSubscriptionManagerSubscription",
            HandleChange,
            cancellationToken);

        var propertyIdagent = CurrentNodeState.PropertyByBrowseName[propertyName].ToIdagentTuple();
        await monitorFactory.MonitorProperty(AgentSubscription, propertyIdagent, cancellationToken);
        await session.Session.PublishAsync(null, new SubscriptionAcknowledgementCollection(), cancellationToken);
        return _subscriptionManager.Subscribe(consumer);
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
                logger.LogInformation("Agent does not have any property named {Item}", item.DisplayName);
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