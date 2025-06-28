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
    private readonly ConcurrentDictionary<Guid, TSubscription> _subscriptions = new();


    public SubscriptionCollection(Func<IMessageConsumer<T>, TSubscription> subscriptionFactory)
    {
        _subscriptionFactory = subscriptionFactory;
    }

    public void Dispose()
    {
        foreach (var su in _subscriptions.Values)
        {
            su.Dispose();
        }
    }

    /// <inheritdoc />
    public int NumberOfSubscriptions => _subscriptions.Count;

    public IEnumerable<IMessageSubscription<T>> Subscriptions =>
        _subscriptions.Values.Select(e => e as IMessageSubscription<T>);

    [Pure]
    public IMessageSubscription<T> Subscribe(IMessageConsumer<T> subscriber)
    {
        var subscription = _subscriptionFactory.Invoke(subscriber);
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