using System;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

internal sealed class ActionStep<TEntity, TArgs>(
    IManagedEntityNode currentState,
    Action<SimulationStepContext<TEntity, TArgs>> simulation,
    Func<IManagedEntityNode, (TEntity toMutate, TEntity safekeep)> copyStateTwice,
    Action<IManagedEntityNode, TEntity, ISystemContext> changeState) : ISimulationStep
{
    public ActionStepResult<TEntity> Execute(TArgs args, ISystemContext context)
    {
        if (Equals(args, default(TArgs)))
        {
            throw new SimulationFailureException(
                $"The arguments of type '{typeof(TArgs).Name}' has not been assigned to the actionStep");
        }

        var (toMutate, history) = copyStateTwice(currentState);
        var simulationContext = new SimulationStepContext<TEntity, TArgs>(toMutate, args);
        simulation.Invoke(simulationContext);
        changeState(currentState, toMutate, context);
        return new ActionStepResult<TEntity>(history, toMutate, DateTime.Now);
    }
}