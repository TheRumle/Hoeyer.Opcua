using System;
using Hoeyer.OpcUa.Simulation.Api.Configuration;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed class SideEffectActionStep<TState, TArguments>(
    Action<SimulationStepContext<TState, TArguments>> sideEffect,
    Func<TState, TState> copy) : ISimulationStep
{
    public void Execute(TState state, TArguments args)
    {
        var context = new SimulationStepContext<TState, TArguments>(copy(state), args);
        sideEffect.Invoke(context);
    }
}