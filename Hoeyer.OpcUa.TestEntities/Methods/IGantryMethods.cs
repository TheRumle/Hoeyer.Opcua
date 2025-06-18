using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Hoeyer.OpcUa.TestEntities.Methods.Generated;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.TestEntities.Methods;

[OpcUaEntityMethods<Gantry>]
public interface IGantryMethods
{
    Task ChangePosition(Position position);
    Task PlaceContainer(Position position);
    Task PickUpContainer(Position position);
    Task<Guid> GetContainerId();
    Task<DateTime> GetDate();
    Task<List<DateTime>> GetDates();
}

public sealed class PlacementSimulator(ILogger<PlacementSimulator> logger)
    : IActionSimulationConfigurator<Gantry, PlaceContainerArgs>,
        IActionSimulationConfigurator<Gantry, PickUpContainerArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IActionSimulationBuilder<Gantry, PickUpContainerArgs> actionSimulationConfiguration)
    {
        return actionSimulationConfiguration
            .Wait(TimeSpan.FromSeconds(2))
            .ChangeState(e =>
            {
                e.State.Position = e.Arguments.Position;
                e.State.Occupied = true;
                e.State.HeldContainer = Guid.NewGuid();
            })
            .Build();
    }

    /// <inheritdoc />
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IActionSimulationBuilder<Gantry, PlaceContainerArgs> actionSimulationConfiguration)
    {
        return actionSimulationConfiguration
            .Wait(TimeSpan.FromSeconds(2))
            .ChangeStateAsync(async e => { await Task.CompletedTask; })
            .Build();
    }
}

public sealed class GetDateSimulator :
    IFunctionSimulationConfigurator<Gantry, GetDateArgs>
{
    /// <inheritdoc />
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IFunctionSimulationBuilder<Gantry, GetDateArgs> functionConfig) =>
        functionConfig.WithReturnValue(e => DateTime.Now);
}