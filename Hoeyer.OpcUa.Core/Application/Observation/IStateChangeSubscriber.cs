namespace Hoeyer.OpcUa.Core.Application.Observation;

public interface IStateChangeSubscriber<in TState>
{
    public void OnStateChange(TState stateChange);
}