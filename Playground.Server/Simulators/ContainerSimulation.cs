using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Playground.Modelling.Methods.Generated;
using Playground.Modelling.Models;

namespace Playground.Server.Simulators;

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