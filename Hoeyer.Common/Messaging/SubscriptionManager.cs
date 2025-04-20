using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging;

public interface ISubscriptionManager<T> : IDisposable
{
    IEnumerable<MessageSubscription> Subscribers { get; }
    MessageSubscription Subscribe(IMessageConsumer<T> subscriber);
    void Publish(T message);
    public void Unpause();
    public void Pause();
}

public sealed class SubscriptionManager<T>(ILogger? logger = null) : IUnsubscribable, IMessageSubscribable<T>, ISubscriptionManager<T>
{
    private readonly ConcurrentDictionary<Guid, MessageSubscription<T>> _subscriptions = new();

    /// <inheritdoc />
    public IEnumerable<MessageSubscription> Subscribers { get; }
    public int NumberOfSubscriptions => _subscriptions.Count;
    private bool _isPaused;
    
    public void Unsubscribe(IMessageSubscription messageSubscription)
    {
        logger?.LogInformation("Removing subscription {Id}", messageSubscription.SubscriptionId.ToString());
        if (!_subscriptions.TryRemove(messageSubscription.SubscriptionId, out _))
        {
            logger?.LogWarning("Failed to remove subscription '{Id}'. Is it already removed?", messageSubscription.SubscriptionId);
        }
    }

    [Pure]
    public MessageSubscription Subscribe(IMessageConsumer<T> subscriber)
    {
        var subscription = new MessageSubscription<T>(this, subscriber);
        if (!_subscriptions.TryAdd(subscription.SubscriptionId, subscription))
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
        if(_isPaused) return;
        var letter = new Message<T>(message);
        foreach (var subscription in _subscriptions.Values)
        {
            if (subscription.IsCancelled || subscription.IsPaused) continue;
            subscription.Forward(letter);
        }
    }

    public void Unpause() => _isPaused = false;

    public void Pause() => _isPaused = true;

    public void Dispose()
    {
        foreach (var su in _subscriptions.Values)
        {
            su.Dispose();
        }
    }
}