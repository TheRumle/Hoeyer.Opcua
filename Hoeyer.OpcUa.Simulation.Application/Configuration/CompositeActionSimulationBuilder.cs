using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Configuration;

internal sealed class CompositeActionSimulationBuilder<TEntity, TArguments, TBuilder>(
    TBuilder builder,
    ILocked<TEntity> lockedEntity,
    IEntityTranslator<TEntity> translator,
    Queue<ISimulationStep> queue)
{
    public TBuilder ChangeState(Action<SimulationStepContext<TEntity, TArguments>> stateChange)
    {
        queue.Enqueue(new MutationStep<TEntity, TArguments>(lockedEntity, stateChange, translator.Copy));
        return builder;
    }

    public TBuilder ChangeStateAsync(Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange)
    {
        queue.Enqueue(new AsyncMutationStep<TEntity, TArguments>(lockedEntity, stateChange, translator.Copy));
        return builder;
    }

    public TBuilder SideEffect(Action<SimulationStepContext<TEntity, TArguments>> sideEffect)
    {
        queue.Enqueue(new SideEffectActionStep<TEntity, TArguments>(lockedEntity, sideEffect, translator.Copy));
        return builder;
    }

    public TBuilder Wait(TimeSpan timeSpan)
    {
        queue.Enqueue(new TimeStep(timeSpan, DateTime.Now));
        return builder;
    }

    public IEnumerable<ISimulationStep> Build() => queue.ToArray();
}