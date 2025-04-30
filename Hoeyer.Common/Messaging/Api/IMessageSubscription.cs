using System;

namespace Hoeyer.Common.Messaging.Api;

public interface IMessageSubscription : IDisposable
{
    Guid SubscriptionId { get; }
    bool IsCancelled { get; }
    bool IsPaused { get; }
    void Unpause();
    void Pause();
}

public interface IMessageSubscription<in T> : IMessageSubscription
{
    void Forward(IMessage<T> message);
}