using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions;

public sealed class SubscriptionCollection<T, TSubscription> : ISubscriptionCollection<T>
    where TSubscription : IMessageSubscription<T>
{
    private readonly Func<IMessageConsumer<T>, TSubscription> _subscriptionFactory;
    private readonly ConcurrentDictionary<Guid, IMessageSubscription<T>> _subscriptions = new();

    public SubscriptionCollection(IMessageSubscriptionFactory<T, TSubscription> factory)
    {
        _subscriptionFactory = consumer => factory.CreateSubscription(consumer, sub => Remove(sub.SubscriptionId));
    }

    /// <inheritdoc />
    public int ActiveSubscriptionsCount => _subscriptions.Values.Count(e => !e.IsCancelled && !e.IsPaused);

    public IEnumerable<IMessageSubscription<T>> Subscriptions => _subscriptions.Values.Select(e => e);

    [Pure]
    public IMessageSubscription<T> Subscribe(IMessageConsumer<T> subscriber)
    {
        IMessageSubscription<T>? subscription = _subscriptionFactory.Invoke(subscriber);
        _subscriptions.TryAdd(subscription.SubscriptionId, subscription);
        return subscription;
    }

    public void Remove(Guid messageSubscription)
    {
        _subscriptions.TryRemove(messageSubscription, out _);
    }

    [Pure]
    public TSubscription CreateSubscriptionFor(IMessageConsumer<T> subscriber)
    {
        var subscription = _subscriptionFactory.Invoke(subscriber);
        _subscriptions.TryAdd(subscription.SubscriptionId, subscription);
        return subscription;
    }
}