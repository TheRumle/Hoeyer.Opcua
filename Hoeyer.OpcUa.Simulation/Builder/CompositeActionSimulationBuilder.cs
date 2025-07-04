using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class CompositeActionSimulationBuilder<TEntity, TArguments, TBuilder>(
    TBuilder builder,
    IManagedEntityNode currentState,
    Queue<ISimulationStep> queue,
    ISimulationStepFactory<TEntity, TArguments> stepFactory) : ISimulationBuilder<TEntity, TArguments, TBuilder>
{
    /// <inheritdoc />
    public TBuilder ChangeState(Action<SimulationStepContext<TEntity, TArguments>> stateChange)
    {
        ActionStep<TEntity, TArguments> step = stepFactory.CreateActionStep(currentState, stateChange);
        queue.Enqueue(step);
        return builder;
    }

    /// <inheritdoc />
    public TBuilder ChangeStateAsync(Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange)
    {
        AsyncActionStep<TEntity, TArguments> action = stepFactory.CreateAsyncActionStep(currentState, stateChange);
        queue.Enqueue(action);
        return builder;
    }

    /// <inheritdoc />
    public TBuilder SideEffect(Action<SimulationStepContext<TEntity, TArguments>> sideEffect)
    {
        var step = stepFactory.CreateSideEffectStep(currentState, sideEffect);
        queue.Enqueue(step);
        return builder;
    }

    /// <inheritdoc />
    public TBuilder Wait(TimeSpan timeSpan)
    {
        TimeStep<TEntity> step = stepFactory.CreateTimeStep(currentState, timeSpan);
        queue.Enqueue(step);
        return builder;
    }

    public IEnumerable<ISimulationStep> Build() => queue.ToArray();
}