namespace Hoeyer.OpcUa.Core.Entity.State;

public interface IStateChangeSubscriber<TState>
{
    public void OnStateChange(StateChange<TState> stateChange);
}