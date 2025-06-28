using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Playground.Models;
using Playground.Models.Methods.Generated;

namespace Playground.Simulation;

public sealed class OnPositionChangeSimulation(ILogger<OnPositionChangeSimulation> logger)
    : IActionSimulationConfigurator<Gantry, ChangePositionArgs>
{
    /// <inheritdoc />  
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IActionSimulationBuilder<Gantry, ChangePositionArgs> onMethodCall)
    {
        return onMethodCall
            .SideEffect((a) => logger.LogInformation("Changing positions to " + a.Arguments.Position))
            .Wait(TimeSpan.FromSeconds(1))
            .ChangeState(e => e.State.Position = e.Arguments.Position)
            .Build();
    }
}