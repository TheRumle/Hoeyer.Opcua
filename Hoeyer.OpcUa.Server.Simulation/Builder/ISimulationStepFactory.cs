using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal interface ISimulationStepFactory<TEntity, TArguments>
{
    /// <param name="currentState">the current state of the <seealso cref="IEntityNode"/> that is to be modified when returned <see cref="ActionStep{TEntity,TArgs}"/> is being executed</param>
    /// <param name="stateChange">the action that will be used to change the state of the <see cref="currentState"/></param>
    /// <param name="executionLock">a lock that prevents concurrent modification of the <see cref="currentState"/></param>
    ActionStep<TEntity, TArguments> CreateActionStep(
        IEntityNode currentState,
        Action<SimulationStepContext<TEntity, TArguments>> stateChange,
        object executionLock);

    public TimeStep<TEntity> CreateTimeStep(IEntityNode node, TimeSpan span, object executionLock);

    public ReturnValueStep<TEntity, TArguments, TReturn> CreateReturnValueStep<TReturn>(IEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueProvider,
        object executionLock);

    public AsyncActionStep<TEntity, TArguments> CreateAsyncActionStep(
        IEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange);
}