namespace Hoeyer.Machines.StateSnapshot;

public interface  IStateChangeSubscriber<TState> 
{
    public void OnStateChange(StateChange<TState> stateChange);
}