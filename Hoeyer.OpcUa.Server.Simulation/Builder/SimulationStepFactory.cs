using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class SimulationStepFactory<TEntity, TArguments>(IEntityTranslator<TEntity> translator)
    : ISimulationStepFactory<TEntity, TArguments>
{
    public ActionStep<TEntity, TArguments> CreateActionStep(
        IManagedEntityNode currentState,
        Action<SimulationStepContext<TEntity, TArguments>> stateChange) =>
        new(currentState, stateChange, CopyStateTwice, ChangeState);

    public ReturnValueStep<TEntity, TArguments, TReturn> CreateReturnValueStep<TReturn>(IManagedEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueProvider) =>
        new(currentState, returnValueProvider, CopyStateTwice, ChangeState);

    public AsyncActionStep<TEntity, TArguments> CreateAsyncActionStep(
        IManagedEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> simulation) =>
        new(currentState, simulation, CopyStateTwice, ChangeState);

    /// <inheritdoc />
    public SideEffectActionStep<TEntity, TArguments> CreateSideEffectStep(
        IManagedEntityNode currentState,
        Action<SimulationStepContext<TEntity, TArguments>> sideEffect)
    {
        var copy = currentState.Select(translator.Translate);
        var simulationContextProvider = (TArguments args) => new SimulationStepContext<TEntity, TArguments>(copy, args);
        return new SideEffectActionStep<TEntity, TArguments>(simulationContextProvider, sideEffect);
    }

    public AsyncSideEffectActionStep<TEntity, TArguments> CreateAsyncSideEffectStep(IManagedEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> sideEffect)
    {
        var copy = currentState.Select(translator.Translate);
        var simulationContextProvider = (TArguments args) => new SimulationStepContext<TEntity, TArguments>(copy, args);
        return new AsyncSideEffectActionStep<TEntity, TArguments>(simulationContextProvider, sideEffect);
    }


    public TimeStep<TEntity> CreateTimeStep(IManagedEntityNode node, TimeSpan span) => new(node.Select(translator.Translate), span, DateTime.Now);

    private void ChangeState(IManagedEntityNode currentState, TEntity state, ISystemContext context)
    {
        currentState.ChangeState(node =>
        {
            node.BaseObject.UpdateChangeMasks(NodeStateChangeMasks.Children | NodeStateChangeMasks.Value);
            translator.AssignToNode(state, node);
            node.BaseObject.ClearChangeMasks(context, true);
        });
    }
    
    private (TEntity toMutate, TEntity safekeep) CopyStateTwice(IManagedEntityNode currentState)
    {
        return currentState
            .Select(node => (translator.Translate(node), translator.Translate(node)));
    }
}