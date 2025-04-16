using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging;

public sealed class SubscriptionManager<T>(ILogger? logger) : IUnsubscribable, IMessageSubscribable<T>
{
    private readonly ConcurrentDictionary<Guid, (IMessageSubscription subscription, IMessageSubscriber<T> subscriber)> _subscriptions = new();

    public IEnumerable<(IMessageSubscription subscription, IMessageSubscriber<T> subscriber)> Subscribers =>
        _subscriptions.Values;
    
    private static readonly string MessageName = typeof(T).Name;
    public int NumberOfSubscriptions => _subscriptions.Count;
    
    public void Unsubscribe(IMessageSubscription messageSubscription)
    {
        logger?.LogInformation("Removing subscription {Id}", messageSubscription.SubscriptionId.ToString());
        if (!_subscriptions.TryRemove(messageSubscription.SubscriptionId, out _))
        {
            logger?.LogWarning("Failed to remove subscription '{Id}'. Is it already removed?", messageSubscription.SubscriptionId);
        }
    }

    [Pure]
    public IMessageSubscription Subscribe(IMessageSubscriber<T> subscriber)
    {
        logger?.BeginScope("Subscribing to messages of type '" + MessageName + '\'');
        var subscription = new MessageSubscription(this);
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

    public void Publish(T message)
    {
        var letter = new Message<T>(message);
        foreach (var (subscription, subscriber) in Subscribers)
        {
            if (subscription.IsCancelled || subscription.IsPaused) continue;
            subscriber.OnMessagePublished(letter);
        }
    }
}