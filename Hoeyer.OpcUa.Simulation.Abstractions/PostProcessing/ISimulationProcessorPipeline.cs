using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Abstractions.Execution;

namespace Hoeyer.OpcUa.Simulation.Abstractions.PostProcessing;

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