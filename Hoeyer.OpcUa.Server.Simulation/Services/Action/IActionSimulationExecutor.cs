using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Action;

internal interface IActionSimulationExecutor<in TArgs>
{
    ValueTask ExecuteSimulation(IEnumerable<ISimulationStep> steps, TArgs args);
}