using Hoeyer.OpcUa.Core.Entity.State;

namespace Hoeyer.OpcUa.Core.Observation;

public interface ISubscribable<TState>
{
    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber);
}