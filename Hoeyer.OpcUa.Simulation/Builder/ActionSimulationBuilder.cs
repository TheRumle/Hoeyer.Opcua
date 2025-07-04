using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class ActionSimulationBuilder<TEntity, TArguments> : IActionSimulationBuilder<TEntity, TArguments>
{
    private readonly
        CompositeActionSimulationBuilder<TEntity, TArguments, IActionSimulationBuilder<TEntity, TArguments>>
        _commonOperations;

    private readonly Queue<ISimulationStep> _simulationSteps;

    public ActionSimulationBuilder(IManagedEntityNode node,
        ISimulationStepFactory<TEntity, TArguments> stepFactory)
    {
        _simulationSteps = new Queue<ISimulationStep>();
        _commonOperations = new(this, node, _simulationSteps, stepFactory);
    }

    public IEnumerable<ISimulationStep> Build() => _simulationSteps.ToArray();

    /// <inheritdoc />
    public IActionSimulationBuilder<TEntity, TArguments> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange) => _commonOperations.ChangeState(stateChange);

    /// <inheritdoc />
    public IActionSimulationBuilder<TEntity, TArguments> ChangeStateAsync(
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange) =>
        _commonOperations.ChangeStateAsync(stateChange);

    /// <inheritdoc />
    public IActionSimulationBuilder<TEntity, TArguments> SideEffect(
        Action<SimulationStepContext<TEntity, TArguments>> sideEffect) => _commonOperations.SideEffect(sideEffect);

    /// <inheritdoc />
    public IActionSimulationBuilder<TEntity, TArguments> Wait(TimeSpan timeSpan) => _commonOperations.Wait(timeSpan);
}