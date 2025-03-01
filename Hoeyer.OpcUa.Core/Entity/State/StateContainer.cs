using Hoeyer.OpcUa.Core.Observation;

namespace Hoeyer.OpcUa.Core.Entity.State;

public sealed class StateContainer<TState>(TState state) : ISubscribable<TState>
{
    private readonly StateChangeBehaviour<TState> _stateChangeBehaviour = new(state);
    public TState State => _stateChangeBehaviour.CurrentState;

    /// <inheritdoc />
    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber)
    {
        return _stateChangeBehaviour.Subscribe(stateChangeSubscriber);
    }

    public void ChangeState(TState newState)
    {
        _stateChangeBehaviour.ChangeState(newState);
    }
}