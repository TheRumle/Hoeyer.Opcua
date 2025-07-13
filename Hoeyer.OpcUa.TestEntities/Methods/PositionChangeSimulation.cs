using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.TestEntities.Methods.Generated;

namespace Hoeyer.OpcUa.TestEntities.Methods;

public sealed class PositionChangeSimulation
    : ISimulation<Gantry, PlaceContainerArgs>,
        ISimulation<Gantry, PickUpContainerArgs>,
        ISimulation<Gantry, ChangePositionArgs>,
        ISimulation<Gantry, GetCurrentContainerIdArgs, Guid>

{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        ISimulationBuilder<Gantry, ChangePositionArgs> onMethodCall)
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
        ISimulationBuilder<Gantry, GetCurrentContainerIdArgs, Guid> config)
    {
        return config.WithReturnValue(t => t.State.HeldContainer);
    }

    public IEnumerable<ISimulationStep> ConfigureSimulation(
        ISimulationBuilder<Gantry, PickUpContainerArgs> onMethodCall)
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
        ISimulationBuilder<Gantry, PlaceContainerArgs> onMethodCall)
    {
        return onMethodCall
            .Wait(TimeSpan.FromSeconds(2))
            .ChangeStateAsync(async e => { await Task.CompletedTask; })
            .Build();
    }
}