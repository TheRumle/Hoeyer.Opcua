namespace Hoeyer.OpcUa.Entity.State;

public interface IStateChangeSubscriber<TState>
{
    public void OnStateChange(StateChange<TState> stateChange);
}