using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

internal sealed class AsyncActionStep<TEntity, TArgs>(IManagedEntityNode currentState,
    Func<SimulationStepContext<TEntity, TArgs>, ValueTask> simulation,
    Func<IManagedEntityNode, (TEntity toMutate, TEntity safekeep)> copyStateTwice,
    Action<IManagedEntityNode, TEntity, ISystemContext> changeState) : ISimulationStep
{

    public async ValueTask<ActionStepResult<TEntity>> Execute(TArgs args, ISystemContext context)
    {
        if (Equals(args, default(TArgs)))
        {
            throw new SimulationFailureException(
                $"The arguments of type '{typeof(TArgs).Name}' has not been assigned to the actionStep");
        }
        
        var (toMutate, history) = copyStateTwice(currentState);
        var state = new SimulationStepContext<TEntity, TArgs>(toMutate, args);
        await simulation.Invoke(state);
        changeState(currentState, toMutate, context);
        return new ActionStepResult<TEntity>(history, state.State, DateTime.Now);
    }
}