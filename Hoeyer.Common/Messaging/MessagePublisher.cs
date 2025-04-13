using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging;

public sealed class MessagePublisher<T>(ILogger? logger = null) : IMessagePublisher<T>
{
    private static readonly string MessageName = typeof(T).Name;
    public int NumberOfSubscriptions => Subscriptions.Count;
    private ConcurrentDictionary<Guid, Subscription<T>> Subscriptions { get; } = new();

    public void Publish(T message)
    {
        var letter = new Message<T>(message);
        foreach (var sub in Subscriptions.Values)
        {
            if (sub.IsCancelled || !sub.IsActive) continue;
            sub.ForwardMessage(letter);
        }
    }

    public void Unsubscribe(Subscription<T> subscription)
    {
        logger?.LogInformation("Removing subscription {Id}", subscription.SubscriptionId.ToString());
        if (!Subscriptions.TryRemove(subscription.SubscriptionId, out _))
        {
            logger?.LogWarning("Failed to remove subscription '{Id}'. Is it already removed?", subscription.SubscriptionId);
        }
    }

    [Pure]
    public Subscription<T> Subscribe(IMessageSubscriber<T> subscriber)
    {
        logger?.BeginScope("Subscribing to messages of type '" + MessageName + '\'');
        var subscription = new Subscription<T>(subscriber, this);
        if (!Subscriptions.TryAdd(subscription.SubscriptionId, subscription))
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