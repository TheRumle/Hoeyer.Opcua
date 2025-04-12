using System;

namespace Hoeyer.Common.Messaging;

public interface ISubscription : IDisposable
{
    bool IsCancelled { get; }
    bool IsActive { get; }
    void Unpause();
    void Pause();
}

public sealed class Subscription<TContent>(IMessageSubscriber<TContent> subscriber) : ISubscription
{
    public readonly Guid SubscriptionId = Guid.NewGuid();
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

    internal void ForwardMessage(IMessage<TContent> stateChange)
    {
        if (IsCancelled || !IsActive)
        {
            return;
        }

        subscriber.OnMessagePublished(stateChange);
    }
}