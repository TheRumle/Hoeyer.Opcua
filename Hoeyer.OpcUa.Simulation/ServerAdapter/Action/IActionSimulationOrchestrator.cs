using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Action;

internal interface IActionSimulationOrchestrator
{
    Task ExecuteMethodSimulation(IList<object> inputArguments, IEnumerable<ISimulationStep> simulationSteps,
        ISystemContext context);
}