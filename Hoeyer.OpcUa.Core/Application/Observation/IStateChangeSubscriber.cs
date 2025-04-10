namespace Hoeyer.OpcUa.Core.Observation;

public interface IStateChangeSubscriber<in TState>
{
    public void OnStateChange(TState stateChange);
}