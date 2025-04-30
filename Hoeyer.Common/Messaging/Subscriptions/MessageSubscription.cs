using System;
using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.Common.Messaging.Subscriptions;

public abstract record MessageSubscription : IMessageSubscription
{
    public Guid SubscriptionId { get; } = Guid.NewGuid();
    private readonly IMessageUnsubscribable _creator;

    protected MessageSubscription(IMessageUnsubscribable creator)
    {
        _creator = creator;
    }

    public bool IsCancelled { get; private set; }
    public bool IsPaused { get; private set;  }

    public void Unpause() => IsPaused = false;
    public void Pause() => IsPaused = true;

    protected virtual void Dispose(bool disposing)
    {
        if (IsCancelled) return;
        if (disposing)
        {
            _creator.Unsubscribe(this);
        }
        IsCancelled = true;

    }

    public void Dispose() => Dispose(true);

    ~MessageSubscription()
    {
        Dispose(false);
    }
}

public sealed record MessageSubscription<T> : MessageSubscription, IMessageSubscription<T>
{
    public MessageSubscription(IMessageUnsubscribable creator, IMessageConsumer<T> consumer) : base(creator)
    {
        _consumer = consumer;
    }

    public void Forward(IMessage<T> message)
    {
        if(IsCancelled) return;
        _consumer.Consume(message);
    }

    private readonly IMessageConsumer<T> _consumer;
}