using System;

namespace Hoeyer.OpcUa.Core.Application.Observation;

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

    internal void ReportStateChange(TState stateChange)
    {
        if (IsCancelled)
        {
            return;
        }

        Subscriber.OnStateChange(stateChange);
    }
}