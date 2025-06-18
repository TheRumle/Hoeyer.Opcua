using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class SimulationStepFactory<TEntity, TArguments>(IEntityTranslator<TEntity> translator)
    : ISimulationStepFactory<TEntity, TArguments>
{
    /// <inheritdoc/>
    public ActionStep<TEntity, TArguments> CreateActionStep(
        IManagedEntityNode currentState,
        Action<SimulationStepContext<TEntity, TArguments>> stateChange)
    {
        Func<TArguments, ActionStepResult<TEntity>> action = args =>
        {
            TEntity previousState = translator.Translate(currentState); //global reference for the current State
            TEntity clonedPrev = translator.Translate(currentState);
            var state = new SimulationStepContext<TEntity, TArguments>(previousState, args);
            stateChange.Invoke(state);
            return new ActionStepResult<TEntity>(clonedPrev, state.State, DateTime.Now);
        };


        return new ActionStep<TEntity, TArguments>(action);
    }

    public ReturnValueStep<TEntity, TArguments, TReturn> CreateReturnValueStep<TReturn>(IManagedEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueProvider)
    {
        Func<TArguments, ReturnValueStepResult<TEntity, TReturn>> action = args =>
        {
            TEntity previousState = translator.Translate(currentState); //global reference for the current State
            TEntity clonedPrev = translator.Translate(currentState);
            var state = new SimulationStepContext<TEntity, TArguments>(previousState, args);
            TReturn? value = returnValueProvider.Invoke(state);
            return new ReturnValueStepResult<TEntity, TReturn>(clonedPrev, state.State, value, DateTime.Now);
        };


        return new ReturnValueStep<TEntity, TArguments, TReturn>(action);
    }

    public AsyncActionStep<TEntity, TArguments> CreateAsyncActionStep(
        IManagedEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange)
    {
        Func<TArguments, Task<ActionStepResult<TEntity>>> action = async args =>
        {
            TEntity previousState = translator.Translate(currentState); //global reference for the current State
            TEntity clonedPrev = translator.Translate(currentState);
            var state = new SimulationStepContext<TEntity, TArguments>(previousState, args);
            await stateChange.Invoke(state);
            return new ActionStepResult<TEntity>(clonedPrev, state.State, DateTime.Now);
        };

        return new AsyncActionStep<TEntity, TArguments>(action);
    }


    public TimeStep<TEntity> CreateTimeStep(IManagedEntityNode node, TimeSpan span)
    {
        TEntity state = translator.Translate(node);
        return new TimeStep<TEntity>(state, span, node.Lock);
    }
}