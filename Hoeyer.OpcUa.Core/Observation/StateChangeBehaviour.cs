using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Hoeyer.OpcUa.Core.Entity.State;

namespace Hoeyer.OpcUa.Core.Observation;

public class StateChangeBehaviour<TState>(TState currentState) : ISubscribable<TState>
{
    private StateChange<TState> _tail = new(currentState, currentState, default!);
    public TState CurrentState => _tail.ReachedState;
    public TState LastState => _tail.PreviousState;

    public DateTime EnteredStateOn => _tail.EnteredStateOn;
    private List<StateChangeSubscription<TState>> Subscriptions { get; set; } = new();


    [Pure]
    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber)
    {
        var subscription = new StateChangeSubscription<TState>(stateChangeSubscriber);
        Subscriptions.Add(subscription);
        return subscription;
    }

    public void ChangeState(TState newState)
    {
        if (Equals(newState, default(TState)))
        {
            throw new ArgumentNullException(nameof(newState));
        }

        Subscriptions = Subscriptions.Where(e => e.IsActive).ToList();
        _tail = new StateChange<TState>(_tail.ReachedState, newState, DateTime.Now);

        foreach (var subscription in Subscriptions) subscription.ReportStateChange(_tail.ReachedState);
    }
}