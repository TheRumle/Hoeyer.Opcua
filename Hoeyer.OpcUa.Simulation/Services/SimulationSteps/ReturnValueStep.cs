using System;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

public sealed class ReturnValueStep<TEntity, TArgs, TReturn>(
    IManagedEntityNode currentState,
    Func<SimulationStepContext<TEntity, TArgs>, TReturn> returnValueProvider,
    Func<IManagedEntityNode, (TEntity toMutate, TEntity safekeep)> copyStateTwice,
    Action<IManagedEntityNode, TEntity, ISystemContext> changeState) : ISimulationStep
{
    public ReturnValueStepResult<TEntity, TReturn> Execute(TArgs args, ISystemContext context)
    {
        if (Equals(args, default(TArgs)))
        {
            throw new SimulationFailureException(
                $"The arguments of type '{typeof(TArgs).Name}' has not been assigned to the actionStep");
        }

        var (toMutate, history) = copyStateTwice(currentState);
        var simulationContext = new SimulationStepContext<TEntity, TArgs>(toMutate, args);
        var value = returnValueProvider.Invoke(simulationContext);
        changeState(currentState, toMutate, context);
        return new ReturnValueStepResult<TEntity, TReturn>(history, simulationContext.State, value, DateTime.Now);
    }
}