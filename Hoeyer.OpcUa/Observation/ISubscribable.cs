using Hoeyer.OpcUa.Entity.State;

namespace Hoeyer.OpcUa.Observation;

public interface ISubscribable<TState>
{
    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber);
}