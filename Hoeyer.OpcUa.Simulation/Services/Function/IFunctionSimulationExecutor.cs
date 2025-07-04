using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Function;

internal interface IFunctionSimulationExecutor<TEntity, in TArgs, TResult>
{
    Task<FunctionSimulationExecutorResult<TEntity, TResult>> ExecuteSimulation(IEnumerable<ISimulationStep> steps,
        TArgs args, ISystemContext systemContext);
}