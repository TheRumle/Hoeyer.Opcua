namespace Hoeyer.OpcUa.StateSnapshot;

public interface  IStateChangeSubscriber<TState> 
{
    public void OnStateChange(StateChange<TState> stateChange);
}