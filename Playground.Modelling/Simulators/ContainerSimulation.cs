using Hoeyer.OpcUa.EntityModelling.Methods.Generated;
using Hoeyer.OpcUa.EntityModelling.Models;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.EntityModelling.Simulators;

public sealed class ContainerSimulation : ISimulation<Gantry, PlaceContainerArgs, int>,
    ISimulation<Gantry, AssignContainerArgs>
{
    private readonly Random _random = new();

    /// <inheritdoc />
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        ISimulationBuilder<Gantry, AssignContainerArgs> onMethodCall)
    {
        return onMethodCall
            .ChangeState(e => e.State.HeldContainer = e.Arguments.Guid)
            .Build();
    }

    public IEnumerable<ISimulationStep> ConfigureSimulation(ISimulationBuilder<Gantry, PlaceContainerArgs, int> config)
    {
        return config.WithReturnValue(state => _random.Next());
    }
}