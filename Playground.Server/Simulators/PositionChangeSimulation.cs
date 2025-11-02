using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Microsoft.Extensions.Logging;
using Playground.Modelling.Methods.Generated;
using Playground.Modelling.Models;

namespace Playground.Server.Simulators;

public sealed class PositionChangeSimulation(ILogger<PositionChangeSimulation> logger)
    : ISimulation<Gantry, ChangePositionArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(ISimulationBuilder<Gantry, ChangePositionArgs> onMethodCall)
    {
        return onMethodCall
            .SideEffect(args => logger.LogDebug("Changing position to {Position}", args.Arguments.Position))
            .Wait(TimeSpan.FromSeconds(1))
            .ChangeState(e => e.State.Position = e.Arguments.Position)
            .Build();
    }
}