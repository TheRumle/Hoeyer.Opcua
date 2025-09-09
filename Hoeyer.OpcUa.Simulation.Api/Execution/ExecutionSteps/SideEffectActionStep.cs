using System;
using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Simulation.Api.Configuration;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed class SideEffectActionStep<TState, TArguments>(
    ILocked<TState> lockedState,
    Action<SimulationStepContext<TState, TArguments>> sideEffect,
    Func<TState, TState> copy) : ISimulationStep
{
    public void Execute(TArguments args)
    {
        var context = new SimulationStepContext<TState, TArguments>(lockedState.Select(copy), args);
        sideEffect.Invoke(context);
    }
}