using Hoeyer.Common.Messaging.Api;
using Hoeyer.Common.Messaging.Subscriptions;

namespace Hoeyer.Common.Messaging;

public class MessagePublisher<T> : IMessagePublisher<T>
{
    public readonly SubscriptionManager<T> SubscriptionManager = new();
    public int NumberOfSubscriptions => SubscriptionManager.Collection.NumberOfSubscriptions;

    public void Publish(T message) => SubscriptionManager.Publish(message);

    public IMessageSubscription<T> Subscribe(IMessageConsumer<T> subscriber) => SubscriptionManager.Collection.Subscribe(subscriber);
}