﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Configuration;

internal sealed class CompositeActionSimulationBuilder<TEntity, TArguments, TBuilder>(
    TBuilder builder,
    IEntityTranslator<TEntity> translator,
    Queue<ISimulationStep> queue)
{
    /// <inheritdoc />
    public TBuilder ChangeState(Action<SimulationStepContext<TEntity, TArguments>> stateChange)
    {
        queue.Enqueue(new MutateStateStep<TEntity, TArguments>(stateChange, translator.Copy));
        return builder;
    }

    /// <inheritdoc />
    public TBuilder ChangeStateAsync(Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange)
    {
        queue.Enqueue(new AsyncActionStep<TEntity, TArguments>(stateChange, translator.Copy));
        return builder;
    }

    /// <inheritdoc />
    public TBuilder SideEffect(Action<SimulationStepContext<TEntity, TArguments>> sideEffect)
    {
        queue.Enqueue(new SideEffectActionStep<TEntity, TArguments>(sideEffect, translator.Copy));
        return builder;
    }

    /// <inheritdoc />
    public TBuilder Wait(TimeSpan timeSpan)
    {
        queue.Enqueue(new TimeStep(timeSpan, DateTime.Now));
        return builder;
    }

    public IEnumerable<ISimulationStep> Build() => queue.ToArray();
}