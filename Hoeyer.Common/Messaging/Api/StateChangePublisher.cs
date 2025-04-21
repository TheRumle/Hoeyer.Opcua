using System.Collections.Generic;
using Hoeyer.Common.Messaging.Subscriptions;

namespace Hoeyer.Common.Messaging.Api;

public sealed class StateChangePublisher<TSubject, TValue> : IStateChangePublisher<TSubject, TValue>
{
    private readonly SubscriptionManager<IEnumerable<StateChange<TSubject, TValue>>> _subscriptionManager = new();
    public IMessageSubscription<IEnumerable<StateChange<TSubject, TValue>>> Subscribe(IMessageConsumer<IEnumerable<StateChange<TSubject, TValue>>> subscriber) => _subscriptionManager.Subscribe(subscriber);
    public void Publish(IEnumerable<StateChange<TSubject, TValue>> message) => _subscriptionManager.Publish(message);
}