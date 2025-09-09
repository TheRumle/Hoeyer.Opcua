using System;
using System.Threading.Tasks;
using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Configuration.Exceptions;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed class AsyncMutationStep<TState, TArgs>(
    ILocked<TState> lockedState,
    Func<SimulationStepContext<TState, TArgs>, ValueTask> mutation,
    Func<TState, TState> copy) : ISimulationStep
{
    public async ValueTask<MutationResult<TState>> Execute(TArgs args)
    {
        if (Equals(args, default(TArgs)))
        {
            throw new SimulationFailureException(
                $"The arguments of type '{typeof(TArgs).Name}' has not been assigned to the actionStep");
        }

        return await lockedState.Select(async state =>
        {
            var reachedState = copy.Invoke(state);
            var simulationContext = new SimulationStepContext<TState, TArgs>(reachedState, args);
            await mutation.Invoke(simulationContext);
            return new MutationResult<TState>(reachedState, DateTime.Now);
        });
    }
}