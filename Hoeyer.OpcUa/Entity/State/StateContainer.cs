using Hoeyer.OpcUa.Observation;

namespace Hoeyer.OpcUa.Entity.State;

public sealed class StateContainer<TState>(TState state) : ISubscribable<TState>
{
    private readonly StateChangeBehaviour<TState> _stateChangeBehaviour = new(state);
    public TState State => _stateChangeBehaviour.CurrentState;
    public void ChangeState(TState newState) => _stateChangeBehaviour.ChangeState(newState);

    /// <inheritdoc />
    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber) => _stateChangeBehaviour.Subscribe(stateChangeSubscriber);
}