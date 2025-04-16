using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging;

public sealed class SubscriptionManager<T>(ILogger? logger) : IUnsubscribable
{
    private readonly ConcurrentDictionary<Guid, (ISubscription subscription, IMessageSubscriber<T> subscriber)> _subscriptions = new();

    public IEnumerable<(ISubscription subscription, IMessageSubscriber<T> subscriber)> Subscribers =>
        _subscriptions.Values;
    
    private static readonly string MessageName = typeof(T).Name;
    public int NumberOfSubscriptions => _subscriptions.Count;
    
    public void Unsubscribe(ISubscription subscription)
    {
        logger?.LogInformation("Removing subscription {Id}", subscription.SubscriptionId.ToString());
        if (!_subscriptions.TryRemove(subscription.SubscriptionId, out _))
        {
            logger?.LogWarning("Failed to remove subscription '{Id}'. Is it already removed?", subscription.SubscriptionId);
        }
    }

    [Pure]
    public ISubscription Subscribe(IMessageSubscriber<T> subscriber)
    {
        logger?.BeginScope("Subscribing to messages of type '" + MessageName + '\'');
        var subscription = new EntitySubscription(this);
        if (!_subscriptions.TryAdd(subscription.SubscriptionId, (subscription, subscriber)))
        {
            logger?.LogError("Failed to add subscription with for {@StateChangeSubscriber}. Messages will not be forwarded...", subscriber);
        }
        else
        {
            logger?.LogError("Added subscription {Id}", subscription.SubscriptionId);
        }
        return subscription;
    }
}