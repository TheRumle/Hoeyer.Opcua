using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.Simulation.Api.Services;

namespace Hoeyer.OpcUa.Simulation.Execution;

internal sealed class SimulationExecutor<TState, TArgs>(ITimeScaler scaler) : ISimulationExecutor<TState, TArgs>
{
    public async IAsyncEnumerable<SimulationResult<TState>>
        ExecuteSimulation(
            TState initialState,
            TArgs args,
            IEnumerable<ISimulationStep> steps)
    {
        var currentState = initialState;
        foreach (ISimulationStep? step in steps)
        {
            if (step is SideEffectActionStep<TState, TArgs> sideEffectActionStep)
            {
                sideEffectActionStep.Execute(currentState, args);
                continue;
            }

            if (step is AsyncSideEffectActionStep<TState, TArgs> asyncSideEffect)
            {
                await asyncSideEffect.Execute(currentState, args);
                continue;
            }


            SimulationResult<TState> result = step switch
            {
                MutateStateStep<TState, TArgs> actionStep => ExecuteMutation(actionStep, currentState, args),
                AsyncActionStep<TState, TArgs> asyncActionStep => await ExecuteAsyncMutation(asyncActionStep,
                    currentState, args),
                TimeStep timeStep => await Sleep(timeStep, currentState),
                ReturnValueStep<TState, TArgs, dynamic> _ => ThrowUseFunctionExecutorInstead(),
                var t => throw new ArgumentOutOfRangeException(t.GetType().Name +
                                                               " is not a handled case for the execution implementation")
            };
            currentState = result.Reached!;
            yield return result;
        }
    }

    private static async Task<SimulationResult<TState>> ExecuteAsyncMutation(
        AsyncActionStep<TState, TArgs> mutateStateStep,
        TState initialState,
        TArgs args)
    {
        var (previousState, reachedState, timeCreated) = await mutateStateStep.Execute(initialState, args);
        return new(previousState, timeCreated, reachedState, ActionType.StateMutation);
    }

    private static SimulationResult<TState> ExecuteMutation(
        MutateStateStep<TState, TArgs> mutateStateStep,
        TState initialState,
        TArgs args)
    {
        var (previousState, reachedState, timeOfMutation) = mutateStateStep.Execute(initialState, args);
        return new(previousState, timeOfMutation, reachedState, ActionType.StateMutation);
    }

    private async Task<SimulationResult<TState>> Sleep(TimeStep timeStep, TState state)
    {
        await timeStep.Execute(scaler);
        return new(state, state, ActionType.Sleep);
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

    public async IAsyncEnumerable<SimulationResult<TState>> ExecuteSimulation(TState initialState, TArgs args,
        IEnumerable<ISimulationStep> steps)
    {
        var executionSteps = steps.ToList();
        validator.ValidateOrThrow(executionSteps);

        TState lastState = initialState;
        await foreach (SimulationResult<TState> step in simulationExecutor.ExecuteSimulation(initialState, args,
                           executionSteps.SkipLast(1)))
        {
            lastState = step.Reached;
            yield return step;
        }

        var returnStep = executionSteps[^1] as ReturnValueStep<TState, TArgs, TReturnValue>;
        var result = returnStep!.Execute(lastState, args);
        Result = result.ReturnValue!;
    }
}