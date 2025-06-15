using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.Function;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class FunctionSimulationBuilder<TEntity, TArguments>(
    IEntityNode node,
    ISimulationStepFactory<TEntity, TArguments> stepFactory) : IFunctionSimulationBuilder<TEntity, TArguments>
{
    private readonly object _lock = new();
    private Queue<ISimulationStep> SimulationSteps { get; } = new();
    private IEntityNode CurrentState => node;


    /// <inheritdoc />
    public IFunctionSimulationBuilder<TEntity, TArguments> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange)
    {
        ActionStep<TEntity, TArguments> step = stepFactory.CreateActionStep(CurrentState, stateChange, _lock);
        SimulationSteps.Enqueue(step);
        return this;
    }

    public IFunctionSimulationBuilder<TEntity, TArguments> Wait(TimeSpan timeSpan)
    {
        TimeStep<TEntity> step = stepFactory.CreateTimeStep(CurrentState, timeSpan, _lock);
        SimulationSteps.Enqueue(step);
        return this;
    }

    /// <inheritdoc />
    public IEnumerable<ISimulationStep> WithReturnValue<TReturn>(
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueFactory)
    {
        ReturnValueStep<TEntity, TArguments, TReturn> step =
            stepFactory.CreateReturnValueStep(CurrentState, returnValueFactory, _lock);
        SimulationSteps.Enqueue(step);
        return SimulationSteps.ToArray();
    }
}