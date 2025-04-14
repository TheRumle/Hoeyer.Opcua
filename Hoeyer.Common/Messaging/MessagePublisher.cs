using Microsoft.Extensions.Logging;

namespace Hoeyer.Common.Messaging;

public class MessagePublisher<T>(ILogger? logger = null) : IMessagePublisher<T>
{
    public readonly SubscriptionManager<T> subscriptionManager = new(logger);
    public int NumberOfSubscriptions => subscriptionManager.NumberOfSubscriptions;

    public void Publish(T message)
    {
        var letter = new Message<T>(message);
        foreach (var (subscription, subscriber) in subscriptionManager.Subscribers)
        {
            if (subscription.IsCancelled || subscription.IsPaused) continue;
            subscriber.OnMessagePublished(letter);
        }
    }

    public Subscription Subscribe(IMessageSubscriber<T> subscriber) => subscriptionManager.Subscribe(subscriber);
}