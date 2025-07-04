using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class
    FunctionSimulationBuilder<TEntity, TArguments, TReturn> : IFunctionSimulationBuilder<TEntity, TArguments, TReturn>
{
    private readonly
        CompositeActionSimulationBuilder<TEntity, TArguments, IFunctionSimulationBuilder<TEntity, TArguments, TReturn>>
        _commonOperations;

    private readonly IManagedEntityNode _node;
    private readonly Queue<ISimulationStep> _simulationSteps;
    private readonly ISimulationStepFactory<TEntity, TArguments> _stepFactory;

    public FunctionSimulationBuilder(IManagedEntityNode node,
        ISimulationStepFactory<TEntity, TArguments> stepFactory)
    {
        _node = node;
        _stepFactory = stepFactory;
        _simulationSteps = new Queue<ISimulationStep>();
        _commonOperations = new(this, _node, _simulationSteps, _stepFactory);
    }


    /// <inheritdoc />
    public IEnumerable<ISimulationStep> WithReturnValue(
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueFactory)
    {
        var step = _stepFactory.CreateReturnValueStep(_node, returnValueFactory);
        _simulationSteps.Enqueue(step);
        return _simulationSteps.ToArray();
    }

    public IEnumerable<ISimulationStep> Build() => _simulationSteps.ToList();

    /// <inheritdoc />
    public IFunctionSimulationBuilder<TEntity, TArguments, TReturn> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange) => _commonOperations.ChangeState(stateChange);

    /// <inheritdoc />
    public IFunctionSimulationBuilder<TEntity, TArguments, TReturn> ChangeStateAsync(
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange) =>
        _commonOperations.ChangeStateAsync(stateChange);

    /// <inheritdoc />
    public IFunctionSimulationBuilder<TEntity, TArguments, TReturn> SideEffect(
        Action<SimulationStepContext<TEntity, TArguments>> sideEffect) => _commonOperations.SideEffect(sideEffect);

    /// <inheritdoc />
    public IFunctionSimulationBuilder<TEntity, TArguments, TReturn> Wait(TimeSpan timeSpan) =>
        _commonOperations.Wait(timeSpan);
}