using System;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class SimulationStepFactory<TEntity, TArguments>(IEntityTranslator<TEntity> translator)
    : ISimulationStepFactory<TEntity, TArguments>
{
    /// <inheritdoc/>
    public ActionStep<TEntity, TArguments> CreateActionStep(
        IEntityNode currentState,
        Action<SimulationStepContext<TEntity, TArguments>> stateChange,
        object executionLock)
    {
        Func<TArguments, ActionStepResult<TEntity>> action = args =>
        {
            TEntity previousState = translator.Translate(currentState); //global reference for the current State
            lock (executionLock)
            {
                TEntity clonedPrev = translator.Translate(currentState);
                var state = new SimulationStepContext<TEntity, TArguments>(previousState, args);
                stateChange.Invoke(state);
                return new ActionStepResult<TEntity>(clonedPrev, state.State, DateTime.Now);
            }
        };


        return new ActionStep<TEntity, TArguments>(action);
    }

    public ReturnValueStep<TEntity, TArguments, TReturn> CreateReturnValueStep<TReturn>(IEntityNode currentState,
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueProvider,
        object executionLock)
    {
        Func<TArguments, ReturnValueStepResult<TEntity, TReturn>> action = args =>
        {
            TEntity previousState = translator.Translate(currentState); //global reference for the current State
            lock (executionLock)
            {
                TEntity clonedPrev = translator.Translate(currentState);
                var state = new SimulationStepContext<TEntity, TArguments>(previousState, args);
                TReturn? value = returnValueProvider.Invoke(state);
                return new ReturnValueStepResult<TEntity, TReturn>(clonedPrev, state.State, value, DateTime.Now);
            }
        };


        return new ReturnValueStep<TEntity, TArguments, TReturn>(action);
    }


    public TimeStep<TEntity> CreateTimeStep(IEntityNode node, TimeSpan span, object executionLock)
    {
        TEntity state = translator.Translate(node);
        return new TimeStep<TEntity>(state, span, executionLock);
    }
}