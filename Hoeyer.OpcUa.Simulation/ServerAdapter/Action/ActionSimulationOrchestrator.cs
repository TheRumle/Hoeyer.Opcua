using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Async;
using Hoeyer.OpcUa.Server.Simulation.Services.Action;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Action;

internal class ActionSimulationOrchestrator<TEntity, TMethodArgs>(
    IMethodArgumentParser<TMethodArgs> inputArgumentParser,
    IActionSimulationExecutor<TEntity, TMethodArgs> executor) : IActionSimulationOrchestrator
{
    public async Task ExecuteMethodSimulation(IList<object> inputArguments,
        IEnumerable<ISimulationStep> simulationSteps, ISystemContext context)
    {
        var argumentStructure = inputArgumentParser.ParseToArgsStructure(inputArguments);
        await executor.ExecuteSimulation(simulationSteps, argumentStructure!, context).Collect();
    }
}