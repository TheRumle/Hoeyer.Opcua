using System;
using Hoeyer.OpcUa.Server.Simulation.Api;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

internal sealed class ActionStep<TEntity, TArgs>(Func<TArgs, ActionStepResult<TEntity>> resultAction)
    : IActionStep<TArgs>
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