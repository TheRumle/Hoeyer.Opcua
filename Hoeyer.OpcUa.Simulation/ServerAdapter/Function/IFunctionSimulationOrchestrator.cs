using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Function;

internal interface IFunctionSimulationOrchestrator<TReturn>
{
    Task<TReturn> ExecuteMethodSimulation(IList<object> inputArguments, IEnumerable<ISimulationStep> simulationSteps,
        ISystemContext context);
}