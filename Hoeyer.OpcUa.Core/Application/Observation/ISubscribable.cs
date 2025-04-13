namespace Hoeyer.OpcUa.Core.Application.Observation;

public interface ISubscribable<TState>
{
    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber);
}