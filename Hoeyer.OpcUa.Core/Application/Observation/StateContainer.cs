using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.OpcUa.Core.Application.Observation;

public class StateContainer<TState>(TState state) : ISubscribable<TState>
{
    public TState State { get; private set; } = state;
    private List<StateChangeSubscription<TState>> Subscriptions { get; set; } = new();

    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber)
    {
        var subscription = new StateChangeSubscription<TState>(stateChangeSubscriber);
        Subscriptions.Add(subscription);
        return subscription;
    }

    public void ChangeState(TState newState)
    {
        State = newState;
        Subscriptions = Subscriptions.Where(e => e.IsActive).ToList();
        foreach (var subscription in Subscriptions) subscription.ReportStateChange(State);
    }
}