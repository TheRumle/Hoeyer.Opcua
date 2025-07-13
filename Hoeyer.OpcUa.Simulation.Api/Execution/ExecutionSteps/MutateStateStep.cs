using System;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Configuration.Exceptions;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed class MutateStateStep<TState, TArgs>(
    Action<SimulationStepContext<TState, TArgs>> mutation,
    Func<TState, TState> copy) : ISimulationStep
{
    public MutationResult<TState> Execute(TState state, TArgs args)
    {
        if (Equals(args, default(TArgs)))
        {
            throw new SimulationFailureException(
                $"The arguments of type '{typeof(TArgs).Name}' has not been assigned to the actionStep");
        }

        var history = copy.Invoke(state);
        var simulationContext = new SimulationStepContext<TState, TArgs>(state, args);
        mutation.Invoke(simulationContext);
        var copiedReachedState = copy.Invoke(simulationContext.State);
        return new MutationResult<TState>(history, copiedReachedState, DateTime.Now);
    }
}