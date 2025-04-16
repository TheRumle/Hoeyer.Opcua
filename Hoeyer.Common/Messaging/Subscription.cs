using System;

namespace Hoeyer.Common.Messaging;

public interface ISubscription : IDisposable
{
    Guid SubscriptionId { get; }
    bool IsCancelled { get; }
    bool IsPaused { get; }
    void Unpause();
    void Pause();
}

public sealed record Subscription : ISubscription
{
    public Guid SubscriptionId { get; } = Guid.NewGuid();
    private readonly IUnsubscribable _creator;

    internal Subscription(IUnsubscribable creator)
    {
        _creator = creator;
    }

    public bool IsCancelled { get; private set; }
    public bool IsPaused { get; private set;  } = false;

    public void Unpause() => IsPaused = false;
    public void Pause() => IsPaused = true;

    public void Dispose()
    {
        if (IsCancelled)
        {
            return;
        }

        _creator.Unsubscribe(this);
        IsCancelled = true;
    }
}