using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Function;

internal record struct FunctionSimulationExecutorResult<TEntity, TResult>(
    IEnumerable<(TEntity Previous, DateTime Time, TEntity Reached)> Transitions,
    TResult ReturnValue);

internal interface IFunctionSimulationExecutor<TEntity, in TArgs, TResult>
{
    Task<FunctionSimulationExecutorResult<TEntity, TResult>> ExecuteSimulation(IEnumerable<ISimulationStep> steps,
        TArgs args, ISystemContext systemContext);
}