using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Simulation.Services.Action;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public interface IActionSimulationConfigurator<TEntity, TArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IActionSimulationBuilder<TEntity, TArgs> actionSimulationConfiguration);
}