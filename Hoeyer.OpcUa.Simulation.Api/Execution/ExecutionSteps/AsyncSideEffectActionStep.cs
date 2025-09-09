using System;
using System.Threading.Tasks;
using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Simulation.Api.Configuration;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed class AsyncSideEffectActionStep<TState, TArguments>(
    ILocked<TState> lockedState,
    Func<SimulationStepContext<TState, TArguments>, ValueTask> sideEffect,
    Func<TState, TState> copy) : ISimulationStep
{
    public async Task Execute(TArguments args)
    {
        await sideEffect(new SimulationStepContext<TState, TArguments>(lockedState.Select(copy), args));
    }
}