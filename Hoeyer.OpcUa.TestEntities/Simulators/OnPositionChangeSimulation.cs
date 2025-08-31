using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.TestEntities.Methods.Generated;
using Hoeyer.OpcUa.TestEntities.Models;

namespace Hoeyer.OpcUa.TestEntities.Simulators;

public sealed class OnPositionChangeSimulation
    : ISimulation<Gantry, ChangePositionArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(ISimulationBuilder<Gantry, ChangePositionArgs> onMethodCall)
    {
        return onMethodCall
            .SideEffect(a => Console.Write("Changing positions to " + a.Arguments.Position))
            .Wait(TimeSpan.FromSeconds(1))
            .ChangeState(e => e.State.Position = e.Arguments.Position)
            .Build();
    }
}