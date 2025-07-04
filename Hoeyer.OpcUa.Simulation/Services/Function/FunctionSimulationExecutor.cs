using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Async;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.Action;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Function;

internal sealed class FunctionSimulationExecutor<TEntity, TArgs, TReturnValue>(ITimeScaler scaler)
    : ActionSimulationExecutor<TEntity, TArgs>(scaler), IFunctionSimulationExecutor<TEntity, TArgs, TReturnValue>
{
    public new async Task<FunctionSimulationExecutorResult<TEntity, TReturnValue>> ExecuteSimulation(
        IEnumerable<ISimulationStep> steps, TArgs args,
        ISystemContext systemContext)
    {
        var executionSteps = steps.ToList();
        AssertSimulationSteps(executionSteps);
        var execution = await base.ExecuteSimulation(executionSteps.SkipLast(1), args, systemContext).Collect();
        var returnStep = executionSteps[^1] as ReturnValueStep<TEntity, TArgs, TReturnValue>;
        var result = returnStep!.Execute(args, systemContext);

        return new FunctionSimulationExecutorResult<TEntity, TReturnValue>(
            [..execution, (result.PreviousState, result.TimeCreated, result.ReachedState)],
            result.ReturnValue);
    }

    private static void AssertSimulationSteps(List<ISimulationStep> executionSteps)
    {
        var errors = LastStepIsReturnValue(executionSteps)
            .Union(OnlyOneReturnValue(executionSteps))
            .ToList();

        if (errors.Count > 0) throw new AggregateException(errors);
    }

    private static IEnumerable<SimulationConfigurationException> OnlyOneReturnValue(
        List<ISimulationStep> executionSteps)
    {
        var earlyReturns = executionSteps.Skip(1)
            .Select((step, index) => (step, index))
            .Where(e => e.step is ReturnValueStep<TEntity, TArgs, TReturnValue>)
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
        List<ISimulationStep> executionSteps)
    {
        if (executionSteps.LastOrDefault() is not ReturnValueStep<TEntity, TArgs, TReturnValue>)
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