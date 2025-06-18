using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class ActionSimulationBuilder<TEntity, TArguments> : IActionSimulationBuilder<TEntity, TArguments>
{
    private readonly CommonSimulationOperations<TEntity, TArguments> _commonOperations;
    private readonly Queue<ISimulationStep> _simulationSteps;

    public ActionSimulationBuilder(IManagedEntityNode node,
        ISimulationStepFactory<TEntity, TArguments> stepFactory)
    {
        _simulationSteps = new Queue<ISimulationStep>();
        _commonOperations = new CommonSimulationOperations<TEntity, TArguments>(node, _simulationSteps, stepFactory);
    }


    public IActionSimulationBuilder<TEntity, TArguments> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange)
    {
        _commonOperations.ChangeState(stateChange);
        return this;
    }

    /// <inheritdoc />
    public IActionSimulationBuilder<TEntity, TArguments> ChangeStateAsync(
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange)
    {
        _commonOperations.ChangeStateAsync(stateChange);
        return this;
    }

    public IActionSimulationBuilder<TEntity, TArguments> SideEffect(Action<TArguments> sideEffect)
    {
        _commonOperations.SideEffect(sideEffect);
        return this;
    }


    public IActionSimulationBuilder<TEntity, TArguments> Wait(TimeSpan timeSpan)
    {
        _commonOperations.Wait(timeSpan);
        return this;
    }

    public IEnumerable<ISimulationStep> Build() => _simulationSteps.ToArray();
}