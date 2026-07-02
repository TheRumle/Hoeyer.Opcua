using System;
using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Simulation.Abstractions.Configuration;

namespace Hoeyer.OpcUa.Simulation.Abstractions.Execution.ExecutionSteps;

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