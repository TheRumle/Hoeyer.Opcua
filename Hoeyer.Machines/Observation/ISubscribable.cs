using Hoeyer.Machines.StateSnapshot;

namespace Hoeyer.Machines.Observation;

public interface ISubscribable<TState>
{
    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber);
}