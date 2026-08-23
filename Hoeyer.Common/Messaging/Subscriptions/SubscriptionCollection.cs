using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions;

public sealed class SubscriptionCollection<T> : ISubscriptionCollection<T>
{
    private readonly Func<IMessageConsumer<T>, IMessageSubscription<T>> _subscriptionFactory;
    private readonly ConcurrentDictionary<Guid, IMessageSubscription<T>> _subscriptions = new();

    public SubscriptionCollection(IMessageSubscriptionFactory<T, IMessageSubscription<T>> factory)
    {
        _subscriptionFactory = consumer => factory.CreateSubscription(consumer, sub => Remove(sub.SubscriptionId));
    }

    /// <inheritdoc />
    public int ActiveSubscriptionsCount => _subscriptions.Values.Count(e => !e.IsCancelled && !e.IsPaused);

    public IEnumerable<IMessageSubscription<T>> Subscriptions => _subscriptions.Values.Select(e => e);

    [Pure]
    public IMessageSubscription<T> Subscribe(IMessageConsumer<T> subscriber)
    {
        var subscription = _subscriptionFactory.Invoke(subscriber);
        _subscriptions.TryAdd(subscription.SubscriptionId, subscription);
        return subscription;
    }

    public void Remove(Guid messageSubscription)
    {
        _subscriptions.TryRemove(messageSubscription, out var _);
    }

    [Pure]
    public IMessageSubscription<T> CreateSubscriptionFor(IMessageConsumer<T> subscriber)
    {
        var subscription = _subscriptionFactory.Invoke(subscriber);
        _subscriptions.TryAdd(subscription.SubscriptionId, subscription);
        return subscription;
    }
}