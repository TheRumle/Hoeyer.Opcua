using System;

namespace Hoeyer.Common.Messaging;

public interface IMessageSubscription : IDisposable
{
    Guid SubscriptionId { get; }
    bool IsCancelled { get; }
    bool IsPaused { get; }
    void Unpause();
    void Pause();
}