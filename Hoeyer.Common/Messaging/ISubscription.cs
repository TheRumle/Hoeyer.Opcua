using System;

namespace Hoeyer.Common.Messaging;

public interface ISubscription : IDisposable
{
    bool IsCancelled { get; }
    bool IsActive { get; }
    void Unpause();
    void Pause();
}