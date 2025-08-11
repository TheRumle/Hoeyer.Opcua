using System.Collections.Generic;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Api.Execution;

public interface ISimulationExecutor<TState, in TArgs>
{
    /// <summary>
    /// Returns 
    /// </summary>
    /// <param name="initialState"></param>
    /// <param name="args"></param>
    /// <param name="steps"></param>
    /// <returns>Async enumerable with the results of the simulation, except side-effects <see cref="SideEffectActionStep{TAgent,TArguments}"/></returns>
    IAsyncEnumerable<SimulationResult<TState>> ExecuteSimulation(TState initialState, TArgs args,
        IEnumerable<ISimulationStep> steps);
}

public interface ISimulationExecutor<TState, in TArgs, out TResult> : ISimulationExecutor<TState, TArgs>
{
    public TResult? Result { get; }
}