using System;
using System.Linq;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions;

public sealed class SubscriptionManager<T> : ISubscriptionManager<T>
{
    private bool _isPaused;
    public  ISubscriptionCollection<T> Collection { get; }
    public SubscriptionManager()
    {
        Collection = new SubscriptionCollection<T, MessageSubscription<T>>(a => new MessageSubscription<T>(this, a));
    }

    public void Publish(T message)
    {
        if(_isPaused) return;
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

    public void Unsubscribe(IMessageSubscription messageSubscription) => Collection.Remove(messageSubscription.SubscriptionId);
    public IMessageSubscription<T> Subscribe(IMessageConsumer<T> subscriber) => Collection.Subscribe(subscriber);

}

public sealed class SubscriptionManager<T, TSubscription> : ISubscriptionManager<T>
where TSubscription : IMessageSubscription<T>
{
    private bool _isPaused;
    public  ISubscriptionCollection<T> Collection { get; }
    public SubscriptionManager(Func<IMessageConsumer<T>, IMessageUnsubscribable, TSubscription> subscriptionFactory)
    {
        Collection = new SubscriptionCollection<T, TSubscription>((a) => subscriptionFactory(a, this));
    }

    public void Publish(T message)
    {
        if(_isPaused) return;
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
    public void Unsubscribe(IMessageSubscription messageSubscription) => Collection.Remove(messageSubscription.SubscriptionId);
    public IMessageSubscription<T> Subscribe(IMessageConsumer<T> subscriber) => Collection.Subscribe(subscriber);
}