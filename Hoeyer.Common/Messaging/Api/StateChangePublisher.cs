using System.Collections.Generic;
using Hoeyer.Common.Messaging.Subscriptions;

namespace Hoeyer.Common.Messaging.Api;
public readonly record struct StateChange<T>(T Previous, T NewValue)
{
    public T Previous { get; } = Previous;
    public T NewValue { get; } = NewValue;
}

public sealed class StateChangePublisher<TSubject, TValue> : IStateChangePublisher<TSubject, TValue>
{
    private readonly SubscriptionManager<Dictionary<TSubject, StateChange<TValue>>> _subscriptionManager = new();
    public IMessageSubscription<Dictionary<TSubject, StateChange<TValue>>> Subscribe(IMessageConsumer<Dictionary<TSubject, StateChange<TValue>>> subscriber) => _subscriptionManager.Subscribe(subscriber);

    /// <inheritdoc />
    public void Publish(Dictionary<TSubject, StateChange<TValue>> message) =>
        _subscriptionManager.Publish(message);
}