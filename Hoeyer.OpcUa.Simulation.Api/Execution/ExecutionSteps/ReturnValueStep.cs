using System;
using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Configuration.Exceptions;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed class ReturnValueStep<TState, TArgs, TReturn>(
    ILocked<TState> lockedState,
    Func<SimulationStepContext<TState, TArgs>, TReturn> returnValueProvider,
    Func<TState, TState> copy) : ISimulationStep
{
    public ReturnValueStepResult<TState, TReturn> Execute(TArgs args)
    {
        if (Equals(args, default(TArgs)))
        {
            throw new SimulationFailureException(
                $"The arguments of type '{typeof(TArgs).Name}' has not been assigned to the actionStep");
        }

        var (prev, next) = lockedState.Select(state => (copy.Invoke(state), copy.Invoke(state)));
        var simulationContext = new SimulationStepContext<TState, TArgs>(prev, args);
        var value = returnValueProvider.Invoke(simulationContext);
        return new ReturnValueStepResult<TState, TReturn>(prev, next, value, DateTime.Now);
    }
}