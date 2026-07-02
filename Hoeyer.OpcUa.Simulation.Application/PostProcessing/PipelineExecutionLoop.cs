using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Abstractions.Execution;
using Hoeyer.OpcUa.Simulation.Abstractions.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.Abstractions.PostProcessing;

namespace Hoeyer.OpcUa.Simulation.PostProcessing;

internal class PipelineExecutionLoop<TState, TArgs>(
    ISimulationProcessorPipeline<TState, TArgs> pipeline,
    ISimulationExecutor<TState, TArgs> executor)
{
    public async Task ExecutePipelineMainLoop(TArgs inputArguments, IEnumerable<ISimulationStep> simulationSteps)
    {
        await pipeline.OnSimulationBegin(inputArguments);
        await foreach (var step in executor.ExecuteSimulation(inputArguments, simulationSteps))
        {
            if (step.Action is ActionType.StateMutation)
            {
                await pipeline.ProcessStateChange(step);
            }
        }
    }
}