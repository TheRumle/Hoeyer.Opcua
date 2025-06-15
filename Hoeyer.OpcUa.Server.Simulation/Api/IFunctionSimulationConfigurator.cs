using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Simulation.Services.Function;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

internal interface IFunctionSimulationConfigurator<TEntity, TArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IFunctionSimulationBuilder<TEntity, TArgs> actionSimulationConfiguration);
}