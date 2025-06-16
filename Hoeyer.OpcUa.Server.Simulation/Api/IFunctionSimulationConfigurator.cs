using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public interface IFunctionSimulationConfigurator<TEntity, TArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IFunctionSimulationBuilder<TEntity, TArgs> functionConfig);
}