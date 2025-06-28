using System.Linq;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions;

public sealed class SubscriptionManager<T> : ISubscriptionManager<T>
{
    private bool _isPaused;

    public SubscriptionManager(IMessageSubscriptionFactory<T> factory)
    {
        Collection = new SubscriptionCollection<T, IMessageSubscription<T>>(consumer =>
        {
            return factory.CreateSubscription(this, consumer);
        });
    }

    public ISubscriptionCollection<T> Collection { get; }

    public void Publish(T message)
    {
        if (_isPaused) return;
        var letter = new Message<T>(message);
        foreach (var subscription in Collection.Subscriptions.ToList())
        {
            if (subscription.IsCancelled || subscription.IsPaused) continue;
            subscription.Forward(letter);
        }
    }

    public void Unpause() => _isPaused = false;

    public void Pause() => _isPaused = true;

    public void Dispose() => Collection.Dispose();

    public void Unsubscribe(IMessageSubscription messageSubscription) =>
        Collection.Remove(messageSubscription.SubscriptionId);

    public IMessageSubscription<T> Subscribe(IMessageConsumer<T> subscriber) => Collection.Subscribe(subscriber);
}

public sealed class SubscriptionManager<T, TSubscription> : ISubscriptionManager<T>
    where TSubscription : IMessageSubscription<T>
{
    private readonly SubscriptionCollection<T, TSubscription> _subscriptionCollection;
    private bool _isPaused;

    public SubscriptionManager(IMessageSubscriptionFactory<T, TSubscription> factory)
    {
        _subscriptionCollection =
            new SubscriptionCollection<T, TSubscription>(consumer => factory.CreateSubscription(this, consumer));
    }

    public ISubscriptionCollection<T> Collection => _subscriptionCollection;

    public void Publish(T message)
    {
        if (_isPaused) return;
        var letter = new Message<T>(message);
        foreach (var subscription in Collection.Subscriptions)
        {
            if (subscription.IsCancelled || subscription.IsPaused) continue;
            subscription.Forward(letter);
        }
    }

    public void Unpause() => _isPaused = false;

    public void Pause() => _isPaused = true;

    public void Dispose() => Collection.Dispose();

    public void Unsubscribe(IMessageSubscription messageSubscription) =>
        Collection.Remove(messageSubscription.SubscriptionId);

    public IMessageSubscription<T> Subscribe(IMessageConsumer<T> subscriber) => Collection.Subscribe(subscriber);

    public TSubscription GetConcreteSubscription(IMessageConsumer<T> subscriber) =>
        _subscriptionCollection.CreateSubscriptionFor(subscriber);
}