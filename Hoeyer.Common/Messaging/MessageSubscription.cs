using System;

namespace Hoeyer.Common.Messaging;

public abstract record MessageSubscription : IMessageSubscription
{
    public Guid SubscriptionId { get; } = Guid.NewGuid();
    private readonly IUnsubscribable _creator;

    protected MessageSubscription(IUnsubscribable creator)
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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    ~MessageSubscription()
    {
        Dispose(false);
    }
}

public sealed record MessageSubscription<T> : MessageSubscription
{
    public MessageSubscription(SubscriptionManager<T> creator, IMessageConsumer<T> consumer) : base(creator)
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