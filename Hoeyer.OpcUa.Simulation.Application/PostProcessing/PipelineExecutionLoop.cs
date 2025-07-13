using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;

namespace Hoeyer.OpcUa.Simulation.PostProcessing;

internal class PipelineExecutionLoop<TState, TArgs>(
    ISimulationProcessorPipeline<TState, TArgs> pipeline,
    ISimulationExecutor<TState, TArgs> executor)
{
    public async Task ExecutePipelineMainLoop(TState initialState, TArgs inputArguments,
        IEnumerable<ISimulationStep> simulationSteps)
    {
        await pipeline.OnSimulationBegin(inputArguments);
        await foreach (var step in executor.ExecuteSimulation(initialState, inputArguments, simulationSteps))
        {
            if (step.Action is ActionType.StateMutation)
            {
                await pipeline.ProcessStateChange(step);
            }
        }
    }
}