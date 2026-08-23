using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions;

public sealed class SubscriptionManager<T>(IMessageSubscriptionFactory<T> factory) : ISubscriptionManager<T>
{
    private bool _isPaused;

    public ISubscriptionCollection<T> Collection { get; } = new SubscriptionCollection<T>(factory);

    public void Publish(T message)
    {
        if (_isPaused)
        {
            return;
        }

        var letter = new Message<T>(message);
        foreach (var subscription in Collection.Subscriptions.ToList())
        {
            if (subscription.IsCancelled)
            {
                Collection.Remove(subscription.SubscriptionId);
                continue;
            }

            if (subscription.IsPaused)
            {
                continue;
            }

            subscription.Forward(letter);
        }
    }

    public void Unpause() => _isPaused = false;

    public void Pause() => _isPaused = true;

    public void Dispose()
    {
        foreach (var subscription in Collection.Subscriptions)
        {
            subscription.Dispose();
        }
    }

    public void Unsubscribe(IMessageSubscription messageSubscription) =>
        Collection.Remove(messageSubscription.SubscriptionId);

    public IMessageSubscription<T> Subscribe(IMessageConsumer<T> subscriber) => Collection.Subscribe(subscriber);
}