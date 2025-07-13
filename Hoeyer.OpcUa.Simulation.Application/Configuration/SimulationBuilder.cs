using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Configuration;

public sealed class SimulationBuilder<TEntity, TArguments> : ISimulationBuilder<TEntity, TArguments>
{
    private readonly
        CompositeActionSimulationBuilder<TEntity, TArguments, SimulationBuilder<TEntity, TArguments>>
        _commonOperations;

    private readonly Queue<ISimulationStep> _simulationSteps;

    public SimulationBuilder(IEntityTranslator<TEntity> translator)
    {
        _simulationSteps = new Queue<ISimulationStep>();
        _commonOperations = new(this, translator, _simulationSteps);
    }

    public IEnumerable<ISimulationStep> Build() => _simulationSteps.ToArray();

    public ISimulationBuilder<TEntity, TArguments> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange) => _commonOperations.ChangeState(stateChange);

    public ISimulationBuilder<TEntity, TArguments> ChangeStateAsync(
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange) =>
        _commonOperations.ChangeStateAsync(stateChange);

    public ISimulationBuilder<TEntity, TArguments> SideEffect(
        Action<SimulationStepContext<TEntity, TArguments>> sideEffect) => _commonOperations.SideEffect(sideEffect);

    public ISimulationBuilder<TEntity, TArguments> Wait(TimeSpan timeSpan) => _commonOperations.Wait(timeSpan);
}

public sealed class
    SimulationBuilder<TEntity, TArguments, TReturn> : ISimulationBuilder<TEntity, TArguments, TReturn>
{
    private readonly CompositeActionSimulationBuilder<TEntity, TArguments,
            ISimulationBuilder<TEntity, TArguments, TReturn>>
        _commonOperations;

    private readonly Queue<ISimulationStep> _simulationSteps = new();
    private readonly IEntityTranslator<TEntity> _translator;

    public SimulationBuilder(IEntityTranslator<TEntity> translator)
    {
        this._translator = translator;
        _commonOperations = new(this, translator, _simulationSteps);
    }


    public IEnumerable<ISimulationStep> WithReturnValue(
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueFactory)
    {
        _simulationSteps.Enqueue(
            new ReturnValueStep<TEntity, TArguments, TReturn>(returnValueFactory, _translator.Copy));
        return _simulationSteps.ToArray();
    }

    public ISimulationBuilder<TEntity, TArguments, TReturn> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange) => _commonOperations.ChangeState(stateChange);

    public ISimulationBuilder<TEntity, TArguments, TReturn> ChangeStateAsync(
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange) =>
        _commonOperations.ChangeStateAsync(stateChange);

    public ISimulationBuilder<TEntity, TArguments, TReturn> SideEffect(
        Action<SimulationStepContext<TEntity, TArguments>> sideEffect) => _commonOperations.SideEffect(sideEffect);

    public ISimulationBuilder<TEntity, TArguments, TReturn> Wait(TimeSpan timeSpan) =>
        _commonOperations.Wait(timeSpan);

    public IEnumerable<ISimulationStep> Build() => _simulationSteps.ToList();
}