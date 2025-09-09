using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.Api.Services;

namespace Hoeyer.OpcUa.Simulation.Execution;

internal sealed class SimulationExecutor<TState, TArgs>(
    ITimeScaler scaler)
    : ISimulationExecutor<TState, TArgs>
{
    public async IAsyncEnumerable<SimulationResult<TState>> ExecuteSimulation(
        TArgs args,
        IEnumerable<ISimulationStep> steps)
    {
        TState _current = default!;
        foreach (ISimulationStep? step in steps)
        {
            if (step is SideEffectActionStep<TState, TArgs> sideEffectActionStep)
            {
                sideEffectActionStep.Execute(args);
                continue;
            }

            if (step is AsyncSideEffectActionStep<TState, TArgs> asyncSideEffect)
            {
                await asyncSideEffect.Execute(args);
                continue;
            }

            var result = step switch
            {
                MutationStep<TState, TArgs> actionStep => ExecuteMutation(_current, actionStep, args),
                AsyncMutationStep<TState, TArgs> asyncActionStep => await ExecuteAsyncMutation(_current,
                    asyncActionStep, args),
                TimeStep timeStep => await Sleep(timeStep, _current),
                ReturnValueStep<TState, TArgs, dynamic> _ => ThrowUseFunctionExecutorInstead(),
                var t => throw new ArgumentOutOfRangeException(t.GetType().Name +
                                                               " is not a handled case for the execution implementation")
            };
            _current = result.Reached;
            yield return result;
        }
    }

    private static async Task<SimulationResult<TState>> ExecuteAsyncMutation(
        TState previousState,
        AsyncMutationStep<TState, TArgs> mutateStateStep,
        TArgs args)
    {
        var (reachedState, timeCreated) = await mutateStateStep.Execute(args);
        return new(previousState, timeCreated, reachedState, ActionType.StateMutation);
    }

    private static SimulationResult<TState> ExecuteMutation(
        TState previousState,
        MutationStep<TState, TArgs> mutateStateStep,
        TArgs args)
    {
        var (reachedState, timeCreated) = mutateStateStep.Execute(args);
        return new SimulationResult<TState>(previousState, timeCreated, reachedState, ActionType.StateMutation);
    }

    private async Task<SimulationResult<TState>> Sleep(TimeStep timeStep, TState state)
    {
        return await timeStep.Execute(scaler)
            .ThenAsync(() => new SimulationResult<TState>(state, state, ActionType.Sleep));
    }

    private static SimulationResult<TState> ThrowUseFunctionExecutorInstead() => throw new ArgumentException(
        $"An action executor cannot handle {typeof(ReturnValueStep<,,>).Name}. Use {typeof(ISimulationExecutor<,,>).Name} with three generic args instead.");
}

internal sealed class SimulationExecutor<TState, TArgs, TReturnValue>(
    ISimulationStepValidator validator,
    ISimulationExecutor<TState, TArgs> simulationExecutor)
    : ISimulationExecutor<TState, TArgs, TReturnValue>
{
    public TReturnValue? Result { get; private set; }

    public async IAsyncEnumerable<SimulationResult<TState>> ExecuteSimulation(
        TArgs args,
        IEnumerable<ISimulationStep> steps)
    {
        var executionSteps = steps.ToList();
        validator.ValidateOrThrow(executionSteps);

        await foreach (SimulationResult<TState> step in simulationExecutor.ExecuteSimulation(args,
                           executionSteps.SkipLast(1)))
        {
            yield return step;
        }

        var returnStep = executionSteps[^1] as ReturnValueStep<TState, TArgs, TReturnValue>;
        var result = returnStep!.Execute(args);
        Result = result.ReturnValue!;
    }
}