using System;
using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Configuration.Exceptions;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed class MutationStep<TState, TArgs>(
    ILocked<TState> lockedState,
    Action<SimulationStepContext<TState, TArgs>> mutation,
    Func<TState, TState> copy) : ISimulationStep
{
    public MutationResult<TState> Execute(TArgs args)
    {
        if (Equals(args, default(TArgs)))
        {
            throw new SimulationFailureException(
                $"The arguments of type '{typeof(TArgs).Name}' has not been assigned to the actionStep");
        }

        return lockedState.Select(state =>
        {
            var next = copy.Invoke(state);
            var simulationContext = new SimulationStepContext<TState, TArgs>(next, args);
            mutation.Invoke(simulationContext);
            return new MutationResult<TState>(next, DateTime.Now);
        });
    }
}