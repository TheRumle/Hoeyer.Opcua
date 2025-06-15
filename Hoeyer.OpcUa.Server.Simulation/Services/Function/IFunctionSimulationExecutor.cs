using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Function;

internal interface IFunctionSimulationExecutor<in TArgs, TReturn>
{
    ValueTask<TReturn> ExecuteSimulation(IEnumerable<ISimulationStep> steps, TArgs args);
}