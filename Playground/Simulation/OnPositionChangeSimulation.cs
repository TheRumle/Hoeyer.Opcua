using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Playground.Models;
using Playground.Models.Methods.Generated;

namespace Playground.Simulation;

public sealed class OnPositionChangeSimulation
    : ISimulation<Gantry, ChangePositionArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(ISimulationBuilder<Gantry, ChangePositionArgs> onMethodCall)
    {
        return onMethodCall
            .SideEffect((a) => Console.Write("Changing positions to " + a.Arguments.Position))
            .Wait(TimeSpan.FromSeconds(1))
            .ChangeState(e => e.State.Position = e.Arguments.Position)
            .Build();
    }
}