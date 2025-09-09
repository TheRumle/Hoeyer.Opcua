using System.Collections.Generic;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Api.Execution;

public interface ISimulationExecutor<TState, in TArgs>
{
    /// <param name="args"></param>
    /// <param name="steps"></param>
    /// <returns>Async enumerable with the results of the simulation, except side-effects <see cref="SideEffectActionStep{TEntity,TArguments}"/></returns>
    IAsyncEnumerable<SimulationResult<TState>> ExecuteSimulation(TArgs args, IEnumerable<ISimulationStep> steps);
}

public interface ISimulationExecutor<TState, in TArgs, out TResult> : ISimulationExecutor<TState, TArgs>
{
    public TResult? Result { get; }
}