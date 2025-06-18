using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Simulation.Api;
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

    /// <inheritdoc />
    public IActionSimulationBuilder<TEntity, TArguments> ChangeStateAsync(
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange)
    {
        AsyncActionStep<TEntity, TArguments> action = stepFactory.CreateAsyncActionStep(CurrentState, stateChange);
        SimulationSteps.Enqueue(action);
        return this;
    }

    public IActionSimulationBuilder<TEntity, TArguments> SideEffect(Action<TArguments> sideEffect)
    {
        SimulationSteps.Enqueue(new SideEffectActionStep<TArguments>(sideEffect));
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