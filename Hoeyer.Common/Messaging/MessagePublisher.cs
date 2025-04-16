using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging;

public class MessagePublisher<T>(ILogger? logger = null) : IMessagePublisher<T>, ISubscribable<T>
{
    public readonly SubscriptionManager<T> SubscriptionManager = new(logger);
    public int NumberOfSubscriptions => SubscriptionManager.NumberOfSubscriptions;

    public void Publish(T message) => SubscriptionManager.Publish(message);

    public IMessageSubscription Subscribe(IMessageSubscriber<T> subscriber) => SubscriptionManager.Subscribe(subscriber);
}