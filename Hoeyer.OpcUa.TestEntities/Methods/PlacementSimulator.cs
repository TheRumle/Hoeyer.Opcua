using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Hoeyer.OpcUa.TestEntities.Methods.Generated;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.TestEntities.Methods;

public sealed class PlacementSimulator(ILogger<PlacementSimulator> logger)
    : IActionSimulationConfigurator<Gantry, PlaceContainerArgs>,
        IActionSimulationConfigurator<Gantry, PickUpContainerArgs>,
        IActionSimulationConfigurator<Gantry, ChangePositionArgs>,
        IFunctionSimulationConfigurator<Gantry, GetCurrentContainerIdArgs, Guid>

{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IActionSimulationBuilder<Gantry, ChangePositionArgs> onMethodCall)
    {
        return onMethodCall
            .Wait(TimeSpan.FromSeconds(3))
            .ChangeState(e =>
            {
                e.State.Position = e.Arguments.Position;
                e.State.Occupied = true;
                e.State.HeldContainer = Guid.NewGuid();
            })
            .Build();
    }

    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IActionSimulationBuilder<Gantry, PickUpContainerArgs> onMethodCall)
    {
        return onMethodCall
            .Wait(TimeSpan.FromSeconds(1))
            .ChangeState(e =>
            {
                e.State.Occupied = true;
                e.State.HeldContainer = Guid.NewGuid();
            })
            .Build();
    }

    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IActionSimulationBuilder<Gantry, PlaceContainerArgs> onMethodCall)
    {
        return onMethodCall
            .Wait(TimeSpan.FromSeconds(2))
            .ChangeStateAsync(async e => { await Task.CompletedTask; })
            .Build();
    }

    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IFunctionSimulationBuilder<Gantry, GetCurrentContainerIdArgs, Guid> functionConfig)
    {
        return functionConfig.WithReturnValue(t => t.State.HeldContainer);
    }
}