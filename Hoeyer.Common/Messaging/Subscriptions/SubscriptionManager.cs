using System.Linq;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions;

public sealed class SubscriptionManager<T>(IMessageSubscriptionFactory<T> factory) : ISubscriptionManager<T>
{
    private bool _isPaused;

    public ISubscriptionCollection<T> Collection { get; } =
        new SubscriptionCollection<T, IMessageSubscription<T>>(factory);

    public void Publish(T message)
    {
        if (_isPaused) return;
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

public sealed class SubscriptionManager<T, TSubscription>(IMessageSubscriptionFactory<T, TSubscription> factory)
    : ISubscriptionManager<T>
    where TSubscription : IMessageSubscription<T>
{
    private readonly SubscriptionCollection<T, TSubscription> _subscriptionCollection = new(factory);
    private bool _isPaused;

    public ISubscriptionCollection<T> Collection => _subscriptionCollection;

    public void Publish(T message)
    {
        if (_isPaused) return;
        var letter = new Message<T>(message);
        var subs = _subscriptionCollection.Subscriptions.ToList();
        foreach (var subscription in subs)
        {
            if (subscription.IsCancelled)
            {
                _subscriptionCollection.Remove(subscription.SubscriptionId);
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
        foreach (var subscription in _subscriptionCollection.Subscriptions)
        {
            subscription.Dispose();
        }
    }

    public void Unsubscribe(IMessageSubscription messageSubscription) =>
        Collection.Remove(messageSubscription.SubscriptionId);

    public IMessageSubscription<T> Subscribe(IMessageConsumer<T> subscriber) => Collection.Subscribe(subscriber);

    public TSubscription GetConcreteSubscription(IMessageConsumer<T> subscriber) =>
        _subscriptionCollection.CreateSubscriptionFor(subscriber);
}