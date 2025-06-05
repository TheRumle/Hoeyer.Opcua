using System;

namespace Hoeyer.Common.Messaging.Api;

public interface ISubscriptionManager<T> : IDisposable, IMessageUnsubscribable, ISubscribable<T>
{
    ISubscriptionCollection<T> Collection { get; }
    void Publish(T message);
    public void Unpause();
    public void Pause();
}

public interface ISubscriptionManager<T, out TSubscription>
    : IDisposable, IMessageUnsubscribable, ISubscribable<T, TSubscription> where TSubscription : IMessageSubscription<T>
{
    ISubscriptionCollection<T> Collection { get; }
    void Publish(T message);
    public void Unpause();
    public void Pause();
}