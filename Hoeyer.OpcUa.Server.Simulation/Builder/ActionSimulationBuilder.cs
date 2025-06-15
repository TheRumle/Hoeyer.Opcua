using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.Action;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class ActionSimulationBuilder<TEntity, TArguments>(
    IEntityNode node,
    ISimulationStepFactory<TEntity, TArguments> stepFactory)
    : IActionSimulationBuilder<TEntity, TArguments>
{
    private readonly object _lock = new();
    private Queue<ISimulationStep> SimulationSteps { get; } = new();
    private IEntityNode CurrentState => node;

    public IActionSimulationBuilder<TEntity, TArguments> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange)
    {
        ActionStep<TEntity, TArguments> step = stepFactory.CreateActionStep(CurrentState, stateChange, _lock);
        SimulationSteps.Enqueue(step);
        return this;
    }

    public IActionSimulationBuilder<TEntity, TArguments> Wait(TimeSpan timeSpan)
    {
        TimeStep<TEntity> step = stepFactory.CreateTimeStep(CurrentState, timeSpan, _lock);
        SimulationSteps.Enqueue(step);
        return this;
    }

    public IEnumerable<ISimulationStep> Build() => SimulationSteps.ToArray();
}