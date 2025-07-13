using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Api.Execution;

namespace Hoeyer.OpcUa.Simulation.Api.PostProcessing;

public interface
    ISimulationProcessorPipeline<TState, in TArgs, in TReturn> : ISimulationProcessorPipeline<TState, TArgs>
{
    public ValueTask OnValueReturned(TReturn producedValue);
}

public interface ISimulationProcessorPipeline<TState, in TArgs>
{
    public ValueTask OnSimulationBegin(TArgs args);
    public ValueTask ProcessStateChange(SimulationResult<TState> stateChange);
    public ValueTask OnSimulationFinished();
}