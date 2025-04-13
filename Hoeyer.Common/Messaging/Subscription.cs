using System;

namespace Hoeyer.Common.Messaging;

public sealed class Subscription<TContent> : ISubscription
{
    public readonly Guid SubscriptionId = Guid.NewGuid();
    private readonly IMessageSubscriber<TContent> _subscriber;
    private readonly IMessagePublisher<TContent> _creator;

    internal Subscription(IMessageSubscriber<TContent> subscriber, IMessagePublisher<TContent> creator)
    {
        _subscriber = subscriber;
        _creator = creator;
    }

    public bool IsCancelled { get; private set; }
    public bool IsActive { get; private set;  } = true;

    public void Unpause() => IsActive = true;
    public void Pause() => IsActive = false;

    public void Dispose()
    {
        if (IsCancelled)
        {
            return;
        }

        _creator.Unsubscribe(this);
        IsCancelled = true;
    }

    internal void ForwardMessage(IMessage<TContent> stateChange)
    {
        if (IsCancelled || !IsActive)
        {
            return;
        }

        _subscriber.OnMessagePublished(stateChange);
    }
}