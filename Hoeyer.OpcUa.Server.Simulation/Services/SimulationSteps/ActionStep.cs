using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Api;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

internal sealed class ActionStep<TEntity, TArgs>(Func<TArgs, ActionStepResult<TEntity>> resultAction)
    : ISimulationStep
{
    public ActionStepResult<TEntity>? Result { get; private set; }

    public void Execute(TArgs args)
    {
        if (Equals(args, default(TArgs)))
        {
            throw new SimulationFailureException(
                $"The arguments of type '{typeof(TArgs).Name}' has not been assigned to the actionStep");
        }

        Result = resultAction.Invoke(args!);
    }
}

internal sealed class SideEffectActionStep<TArguments>(Action<TArguments> sideEffect) : ISimulationStep
{
    public void Execute(TArguments args)
    {
        sideEffect.Invoke(args);
    }
}

internal sealed class AsyncActionStep<TEntity, TArgs>(Func<TArgs, Task<ActionStepResult<TEntity>>> resultAction)
    : ISimulationStep
{
    public ActionStepResult<TEntity> Result { get; set; }

    public async ValueTask Execute(TArgs args)
    {
        if (Equals(args, default(TArgs)))
        {
            throw new SimulationFailureException(
                $"The arguments of type '{typeof(TArgs).Name}' has not been assigned to the actionStep");
        }

        Result = await resultAction.Invoke(args!);
    }
}