using System;

namespace Hoeyer.Machines.StateSnapshot;

public sealed class StateChangeSubscription<TState>(IStateChangeSubscriber<TState> subscriber) : IDisposable
{
    internal readonly IStateChangeSubscriber<TState> Subscriber = subscriber;
    public bool IsCancelled { get; private set; }
    public bool IsActive => !IsCancelled;

    public void Dispose()
    {
        if (!IsCancelled)
        {
            IsCancelled = true;
        }
    }

    internal void ReportStateChange(StateChange<TState> stateChange)
    {
        if (IsCancelled) return;
        Subscriber.OnStateChange(stateChange);
    }
}