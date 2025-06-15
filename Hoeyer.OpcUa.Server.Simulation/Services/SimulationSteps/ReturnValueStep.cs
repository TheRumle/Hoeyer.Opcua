using System;
using Hoeyer.OpcUa.Server.Simulation.Api;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

public sealed class ReturnValueStep<TEntity, TArgs, TReturn>(
    Func<TArgs, ReturnValueStepResult<TEntity, TReturn>> resultAction) : IActionStep<TArgs>
{
    public ReturnValueStepResult<TEntity, TReturn>? Result { get; private set; }

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