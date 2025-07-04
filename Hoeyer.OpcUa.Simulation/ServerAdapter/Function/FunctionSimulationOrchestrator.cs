using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Services.Function;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Function;

internal class FunctionSimulationOrchestrator<TEntity, TMethodArgs, TReturn>(
    IMethodArgumentParser<TMethodArgs> inputArgumentParser,
    IFunctionSimulationExecutor<TEntity, TMethodArgs, TReturn> executor) : IFunctionSimulationOrchestrator<TReturn>
{
    public async Task<TReturn> ExecuteMethodSimulation(IList<object> inputArguments,
        IEnumerable<ISimulationStep> simulationSteps, ISystemContext context)
    {
        var argumentStructure = inputArgumentParser.ParseToArgsStructure(inputArguments);
        var res = await executor.ExecuteSimulation(simulationSteps, argumentStructure!, context);
        return res.ReturnValue;
    }
}