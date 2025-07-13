using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Api.Configuration;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed class AsyncSideEffectActionStep<TState, TArguments>(
    Func<SimulationStepContext<TState, TArguments>, ValueTask> sideEffect,
    Func<TState, TState> copy) : ISimulationStep
{
    public async Task Execute(TState state, TArguments args)
    {
        await sideEffect(new SimulationStepContext<TState, TArguments>(copy(state), args));
    }
}