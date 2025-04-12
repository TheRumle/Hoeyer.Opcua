using System;

namespace Hoeyer.Common.Messaging;

public sealed class Subscription<TContent>(IMessageSubscriber<TContent> subscriber) : IDisposable
{
    public readonly Guid SubscriptionId = Guid.NewGuid();
    private IMessageSubscriber<TContent> _subscriber = subscriber;
    public bool IsCancelled { get; private set; }
    public bool IsActive { get; private set;  } = true;

    public void Unpause() => IsActive = true;
    public void Pause() => IsActive = false;

    public void Dispose()
    {
        if (!IsCancelled)
        {
            IsCancelled = true;
        }
    }

    internal void ForwardMessage(TContent stateChange)
    {
        if (IsCancelled || !IsActive)
        {
            return;
        }

        _subscriber.OnMessagePublished(stateChange);
    }
}