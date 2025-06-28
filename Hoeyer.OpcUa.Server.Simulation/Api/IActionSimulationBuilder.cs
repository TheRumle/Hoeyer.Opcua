using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public interface IActionSimulationBuilder<TEntity, TArguments> : ISimulationBuilder<TEntity, TArguments, IActionSimulationBuilder<TEntity, TArguments>>
{
    IEnumerable<ISimulationStep> Build();
}