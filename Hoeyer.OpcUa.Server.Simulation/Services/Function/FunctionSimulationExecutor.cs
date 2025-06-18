using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.Action;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Function;

internal sealed class FunctionSimulationExecutor<TEntity, TArgs, TReturnValue>(ITimeScaler scaler)
    : ActionSimulationExecutor<TEntity, TArgs>(scaler), IFunctionSimulationExecutor<TArgs, TReturnValue>
{
    public new async ValueTask<TReturnValue> ExecuteSimulation(IEnumerable<ISimulationStep> steps, TArgs args)
    {
        List<ISimulationStep> executionSteps = steps.ToList();
        AssertSimulationSteps(executionSteps);
        await base.ExecuteSimulation(executionSteps.SkipLast(1), args);
        var returnStep = executionSteps[^1] as ReturnValueStep<TEntity, TArgs, TReturnValue>;
        returnStep!.Execute(args);
        return returnStep.Result!.ReturnValue;
    }


    private static void AssertSimulationSteps(List<ISimulationStep> executionSteps)
    {
        List<SimulationConfigurationException> errors =
            LastStepIsReturnValue(executionSteps).Union(OnlyOneReturnValue(executionSteps)).ToList();
        if (errors.Count > 0) throw new AggregateException(errors);
    }

    private static IEnumerable<SimulationConfigurationException> OnlyOneReturnValue(
        List<ISimulationStep> executionSteps)
    {
        List<(ISimulationStep step, int index)> earlyReturns = executionSteps
            .Select((step, index) => (step, index))
            .Where(e => e.step is ReturnValueStep<TEntity, TArgs, TReturnValue>)
            .ToList();


        if (earlyReturns.Count > 0)
        {
            var indices = string.Join("\n", earlyReturns.Select(e => e.index));
            return
            [
                new SimulationConfigurationException(
                    $"The simulation contains return value step(s) in wrong execution positions. A function simulation must contain only one step that returns a value and it must be the last step of the simulation.\n The simulation had return values configured at indices [{indices}]")
            ];
        }

        return [];
    }

    private static IEnumerable<SimulationConfigurationException> LastStepIsReturnValue(
        List<ISimulationStep> executionSteps)
    {
        ISimulationStep? returnStep = executionSteps.LastOrDefault();
        if (returnStep == null)
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