using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class FunctionSimulationBuilder<TEntity, TArguments, TReturn>(
    IEntityNode node,
    ISimulationStepFactory<TEntity, TArguments> stepFactory)
    : IFunctionSimulationBuilder<TEntity, TArguments, TReturn>
{
    private readonly object _lock = new();
    private Queue<ISimulationStep> SimulationSteps { get; } = new();
    private IEntityNode CurrentState => node;

    /// <inheritdoc />
    public IEnumerable<ISimulationStep> WithReturnValue(
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueFactory)
    {
        ReturnValueStep<TEntity, TArguments, TReturn> step =
            stepFactory.CreateReturnValueStep(CurrentState, returnValueFactory, _lock);
        SimulationSteps.Enqueue(step);
        return SimulationSteps.ToArray();
    }


    public IFunctionSimulationBuilder<TEntity, TArguments, TReturn> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange)
    {
        ActionStep<TEntity, TArguments> step = stepFactory.CreateActionStep(CurrentState, stateChange, _lock);
        SimulationSteps.Enqueue(step);
        return this;
    }

    public IFunctionSimulationBuilder<TEntity, TArguments, TReturn> Wait(TimeSpan timeSpan)
    {
        TimeStep<TEntity> step = stepFactory.CreateTimeStep(CurrentState, timeSpan, _lock);
        SimulationSteps.Enqueue(step);
        return this;
    }
}