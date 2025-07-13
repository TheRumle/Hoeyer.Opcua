using System;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Configuration.Exceptions;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed class ReturnValueStep<TState, TArgs, TReturn>(
    Func<SimulationStepContext<TState, TArgs>, TReturn> returnValueProvider,
    Func<TState, TState> copy) : ISimulationStep
{
    public ReturnValueStepResult<TState, TReturn> Execute(TState state, TArgs args)
    {
        if (Equals(args, default(TArgs)))
        {
            throw new SimulationFailureException(
                $"The arguments of type '{typeof(TArgs).Name}' has not been assigned to the actionStep");
        }

        var history = copy.Invoke(state);
        var simulationContext = new SimulationStepContext<TState, TArgs>(state, args);
        var value = returnValueProvider.Invoke(simulationContext);
        return new ReturnValueStepResult<TState, TReturn>(history, simulationContext.State, value, DateTime.Now);
    }
}