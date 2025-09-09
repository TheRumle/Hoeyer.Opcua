using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.PostProcessing;

namespace Hoeyer.OpcUa.Simulation.Execution;

internal sealed class SimulationOrchestrator<TState, TMethodArgs>(
    ISimulationProcessorPipeline<TState, TMethodArgs> pipeline,
    ISimulationExecutor<TState, TMethodArgs> executor)
    : ISimulationOrchestrator<TState, TMethodArgs>
{
    public async Task ExecuteMethodSimulation(TMethodArgs inputArguments,
        IEnumerable<ISimulationStep> simulationSteps)
    {
        var loop = new PipelineExecutionLoop<TState, TMethodArgs>(pipeline, executor);
        await loop.ExecutePipelineMainLoop(inputArguments, simulationSteps);
        await pipeline.OnSimulationFinished();
    }
}

internal class SimulationOrchestrator<TState, TMethodArgs, TReturn>(
    ISimulationProcessorPipeline<TState, TMethodArgs, TReturn> pipeline,
    ISimulationExecutor<TState, TMethodArgs, TReturn> executor) : ISimulationOrchestrator<TState, TMethodArgs, TReturn>
{
    public async Task<TReturn> ExecuteMethodSimulation(TMethodArgs inputArguments,
        IEnumerable<ISimulationStep> simulationSteps)
    {
        await ((ISimulationOrchestrator<TState, TMethodArgs>)this).ExecuteMethodSimulation(inputArguments,
            simulationSteps);
        return executor.Result!;
    }

    /// <inheritdoc />
    async Task ISimulationOrchestrator<TState, TMethodArgs>.ExecuteMethodSimulation(TMethodArgs inputArguments,
        IEnumerable<ISimulationStep> simulationSteps)
    {
        var loop = new PipelineExecutionLoop<TState, TMethodArgs>(pipeline, executor);
        await loop.ExecutePipelineMainLoop(inputArguments, simulationSteps);
        if (executor.Result is not null)
        {
            await pipeline.OnValueReturned(executor.Result);
        }

        await pipeline.OnSimulationFinished();
    }
}