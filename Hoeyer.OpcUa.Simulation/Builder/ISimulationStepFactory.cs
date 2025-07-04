using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal interface ISimulationStepFactory<TEntity, TArguments>
{
    /// <param name="currentState">the current state of the <seealso cref="IEntityNode"/> that is to be modified when returned <see cref="ActionStep{TEntity,TArgs}"/> is being executed</param>
    /// <param name="stateChange">the action that will be used to change the state of the <see cref="currentState"/></param>
    ActionStep<TEntity, TArguments> CreateActionStep(
        IManagedEntityNode currentState,
        Action<SimulationStepContext<TEntity, TArguments>> stateChange);

    public TimeStep<TEntity> CreateTimeStep(IManagedEntityNode node, TimeSpan span);

    public ReturnValueStep<TEntity, TArguments, TReturn> CreateReturnValueStep<TReturn>(IManagedEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueProvider);

    public AsyncActionStep<TEntity, TArguments> CreateAsyncActionStep(
        IManagedEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> simulation);

    public SideEffectActionStep<TEntity, TArguments> CreateSideEffectStep(
        IManagedEntityNode currentState,
        Action<SimulationStepContext<TEntity, TArguments>> sideEffect);

    public AsyncSideEffectActionStep<TEntity, TArguments> CreateAsyncSideEffectStep(
        IManagedEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> sideEffect);
}