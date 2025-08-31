using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.TestEntities.Methods.Generated;
using Hoeyer.OpcUa.TestEntities.Models;

namespace Hoeyer.OpcUa.TestEntities.Simulators;

public sealed class GetCurrentPositionSimulation
    : ISimulation<Gantry, GetCurrentContainerIdArgs, Guid>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        ISimulationBuilder<Gantry, GetCurrentContainerIdArgs, Guid> config)
    {
        return config
            .Wait(TimeSpan.FromMilliseconds(20))
            .WithReturnValue(_ => Guid.CreateVersion7());
    }
}