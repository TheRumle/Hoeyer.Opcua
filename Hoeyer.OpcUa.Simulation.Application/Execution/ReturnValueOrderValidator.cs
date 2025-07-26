using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Simulation.Api.Configuration.Exceptions;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Execution;

internal sealed class ReturnValueOrderValidator<TState, TArgs, TReturnValue> : ISimulationStepValidator
{
    /// <inheritdoc />
    public void ValidateOrThrow(IEnumerable<ISimulationStep> steps)
    {
        var list = steps.ToList();
        var errors = LastStepIsReturnValue(list)
            .Union(OnlyOneReturnValue(list))
            .ToList();

        if (errors.Count > 0) throw new AggregateException(errors);
    }

    private static IEnumerable<SimulationConfigurationException> OnlyOneReturnValue(
        List<ISimulationStep> executionSteps)
    {
        var earlyReturns = executionSteps.Skip(1)
            .Select((step, index) => (step, index))
            .Where(e => e.step is ReturnValueStep<TState, TArgs, TReturnValue>)
            .ToList();


        if (earlyReturns.Count > 0)
        {
            var indices = string.Join("\n", earlyReturns.Select(e => e.index));
            return
            [
                new SimulationConfigurationException(
                    $"The simulation contains return value step(s) in wrong execution positions. A function simulation must contain only one step that returns a value and it must be the last step of the simulation.\n The simulation had return values configured at indices [{indices}] but should only be placed at index {executionSteps.Count - 1}")
            ];
        }

        return [];
    }

    private static IEnumerable<SimulationConfigurationException> LastStepIsReturnValue(
        IEnumerable<ISimulationStep> executionSteps)
    {
        if (executionSteps.LastOrDefault() is not ReturnValueStep<TState, TArgs, TReturnValue>)
        {
            return
            [
                new SimulationConfigurationException(
                    $"The simulation is wrongly configured. Expected last step in simulation to be of type '{typeof(ReturnValueStep<int, int, int>).Name}'")
            ];
        }

        return [];
    }
}