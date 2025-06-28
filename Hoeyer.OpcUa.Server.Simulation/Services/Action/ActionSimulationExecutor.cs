using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.Function;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Action;

internal class ActionSimulationExecutor<TEntity, TArgs>(ITimeScaler scaler) : IActionSimulationExecutor<TEntity, TArgs>
{
    /// <exception cref="ArgumentException"></exception>
    /// <inheritdoc />
    public virtual async IAsyncEnumerable<(TEntity Previous, DateTime Time, TEntity Reached)> ExecuteSimulation(
        IEnumerable<ISimulationStep> steps, TArgs args, ISystemContext systemContext)
    {
        foreach (ISimulationStep? step in steps)
        {
            if (step is SideEffectActionStep<TEntity, TArgs> sideEffectActionStep)
            {
                sideEffectActionStep.Execute(args);
                continue;
            }

            if (step is AsyncSideEffectActionStep<TEntity, TArgs> asyncSideEffect)
            {
                await asyncSideEffect.Execute(args);
                continue;
            }

            yield return step switch
            {
                ActionStep<TEntity, TArgs> actionStep => Execute(actionStep, args, systemContext),
                AsyncActionStep<TEntity, TArgs> asyncActionStep => await Execute(asyncActionStep, args, systemContext),
                TimeStep<TEntity> timeStep => await Sleep(timeStep),
                ReturnValueStep<TEntity, TArgs, dynamic> _ => ThrowUseFunctionExecutorInstead(),
                var t => throw new ArgumentOutOfRangeException(t.GetType().Name)
            };
        }
    }

    private static (TEntity, DateTime, TEntity) ThrowUseFunctionExecutorInstead() => throw new ArgumentException(
        $"An action executor cannot handle {typeof(ReturnValueStep<,,>).Name}. Use {typeof(IFunctionSimulationExecutor<,,>).Name} instead.");

    private static async Task<(TEntity Previous, DateTime Time, TEntity Reached)> Execute(
        AsyncActionStep<TEntity, TArgs> asyncActionStep, TArgs args, ISystemContext systemContext)
    {
        var (previousState, reachedState, timeCreated) = await asyncActionStep.Execute(args, systemContext);
        return (previousState, timeCreated, reachedState);
    }

    private static (TEntity Previous, DateTime Time, TEntity Reached) Execute(ActionStep<TEntity, TArgs> actionStep,
        TArgs args, ISystemContext systemContext)
    {
        var (previousState, reachedState, timeCreated) = actionStep.Execute(args, systemContext);
        return (previousState, timeCreated, reachedState);
    }

    private async Task<(TEntity, DateTime, TEntity)> Sleep(TimeStep<TEntity> timeStep)
    {
        var scaledTime = scaler.ScaleDown(timeStep.TimeSpan);
        await Task.Delay(scaledTime);
        return (timeStep.State, timeStep.CreationTime + timeStep.TimeSpan, timeStep.State);
    }
}