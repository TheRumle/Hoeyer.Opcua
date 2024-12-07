using Hoeyer.Machines.StateSnapshot;

namespace Hoeyer.Machines.Machine;

public sealed class Machine<TState>(TState state) : ISubscribable<TState>
{
    private readonly StateChangeBehaviour<TState> _stateChangeBehaviour = new(state);
    public TState State => _stateChangeBehaviour.CurrentState;
    public void ChangeState(TState newState) => _stateChangeBehaviour.ChangeState(newState);

    /// <inheritdoc />
    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber) => _stateChangeBehaviour.Subscribe(stateChangeSubscriber);
}