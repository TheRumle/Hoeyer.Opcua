using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.PostProcessing;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.Execution;

internal sealed class SimulationOrchestrator<TState, TMethodArgs>(
    IServiceProvider serviceProvider,
    ISimulationExecutor<TState, TMethodArgs> executor)
    : ISimulationOrchestrator<TState, TMethodArgs>
{
    public async Task ExecuteMethodSimulation(TState initialState, TMethodArgs inputArguments,
        IEnumerable<ISimulationStep> simulationSteps)
    {
        await using var pipelineScope = serviceProvider.CreateAsyncScope();
        var pipeline = pipelineScope.ServiceProvider
            .GetRequiredService<ISimulationProcessorPipeline<TState, TMethodArgs>>();
        var loop = new PipelineExecutionLoop<TState, TMethodArgs>(pipeline, executor);
        await loop.ExecutePipelineMainLoop(initialState, inputArguments, simulationSteps);
        await pipeline.OnSimulationFinished();
    }
}

internal class SimulationOrchestrator<TState, TMethodArgs, TReturn>(
    IServiceProvider serviceProvider,
    ISimulationExecutor<TState, TMethodArgs, TReturn> executor) : ISimulationOrchestrator<TState, TMethodArgs, TReturn>
{
    public async Task<TReturn> ExecuteMethodSimulation(TState initialState, TMethodArgs inputArguments,
        IEnumerable<ISimulationStep> simulationSteps)
    {
        await (this as ISimulationOrchestrator<TState, TMethodArgs>).ExecuteMethodSimulation(initialState,
            inputArguments, simulationSteps);
        return executor.Result!;
    }

    /// <inheritdoc />
    async Task ISimulationOrchestrator<TState, TMethodArgs>.ExecuteMethodSimulation(TState initialState,
        TMethodArgs inputArguments, IEnumerable<ISimulationStep> simulationSteps)
    {
        await using var pipelineScope = serviceProvider.CreateAsyncScope();
        var pipeline = pipelineScope.ServiceProvider
            .GetRequiredService<ISimulationProcessorPipeline<TState, TMethodArgs, TReturn>>();

        var loop = new PipelineExecutionLoop<TState, TMethodArgs>(pipeline, executor);
        await loop.ExecutePipelineMainLoop(initialState, inputArguments, simulationSteps);
        if (executor.Result is not null)
        {
            await pipeline.OnValueReturned(executor.Result);
        }

        await pipeline.OnSimulationFinished();
    }
}