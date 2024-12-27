using Hoeyer.OpcUa.StateSnapshot;

namespace Hoeyer.OpcUa.Observation;

public interface ISubscribable<TState>
{
    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber);
}