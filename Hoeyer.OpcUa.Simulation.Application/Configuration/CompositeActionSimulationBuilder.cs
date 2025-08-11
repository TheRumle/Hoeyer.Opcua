using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Configuration;

internal sealed class CompositeActionSimulationBuilder<TAgent, TArguments, TBuilder>(
    TBuilder builder,
    IAgentTranslator<TAgent> translator,
    Queue<ISimulationStep> queue)
{
    /// <inheritdoc />
    public TBuilder ChangeState(Action<SimulationStepContext<TAgent, TArguments>> stateChange)
    {
        queue.Enqueue(new MutateStateStep<TAgent, TArguments>(stateChange, translator.Copy));
        return builder;
    }

    /// <inheritdoc />
    public TBuilder ChangeStateAsync(Func<SimulationStepContext<TAgent, TArguments>, ValueTask> stateChange)
    {
        queue.Enqueue(new AsyncActionStep<TAgent, TArguments>(stateChange, translator.Copy));
        return builder;
    }

    /// <inheritdoc />
    public TBuilder SideEffect(Action<SimulationStepContext<TAgent, TArguments>> sideEffect)
    {
        queue.Enqueue(new SideEffectActionStep<TAgent, TArguments>(sideEffect, translator.Copy));
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