using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;

namespace Hoeyer.OpcUa.Simulation.Api.Execution;

/// <summary>
/// Orchestrates simulations of action executions. Is responsible for coordinating the execution of a simulated action and maintaining a pipeline for plug-in side-effects defined by implementations of <see cref="IStateChangeSimulationProcessor{TState}"/>. A new pipeline and plug-in instances are created for each invocation of the simulation execution.  
/// <list type="bullet">
///   <item>
///     <description>The simulated behaviour is defined through an implementation of <see cref="ISimulation{TEntity,TArgs}"/></description>
///   </item>
///   <item>
///     <description>The defined simulation is then executed using an <see cref="ISimulationExecutor{TState,TArgs}"/>, and a <see cref="ISimulationProcessorPipeline{TState,TArgs}"/> is called at the beginning of the simulation, for each successful execution of a <see cref="ISimulationStep"/> as well as when the simulation is finished</description>
///   </item>
/// </list>
/// </summary>
/// <typeparam name="TArgs">A container containing the arguments of the simulated action</typeparam>
/// <typeparam name="TState">The state related to the simulation</typeparam>
public interface ISimulationOrchestrator<in TState, in TArgs>
{
    /// <summary>
    /// Executes the 
    /// </summary>
    /// <param name="initialState">The initial state when the simulation begins</param>
    /// <param name="inputArguments">A container containing the arguments of the simulated action</param>
    /// <param name="simulationSteps">The steps that must be executed before the simulation is finished - <see cref="ISimulationStep"/></param>
    /// <returns></returns>
    Task ExecuteMethodSimulation(TState initialState, TArgs inputArguments,
        IEnumerable<ISimulationStep> simulationSteps);
}

/// <summary>
/// Orchestrates simulations of function executions. Is responsible for coordinating the execution of a simulated function and maintaining a pipeline for plug-in side-effects defined by implementations of <see cref="IFunctionSimulationProcessor{TEntity,TArgs,TReturn}"/>. A new pipeline and plug-in instances are created for each invocation of the simulation execution.  
/// <list type="bullet">
///   <item>
///     <description>The simulated behaviour is defined through an implementation of <see cref="ISimulation{TEntity,TArgs,TReturn}"/></description>
///   </item>
///   <item>
///     <description>The defined simulation is then executed using an <see cref="Hoeyer.OpcUa.Simulation.Api.Execution.ISimulationExecutor{TEntity, TArgs, TResult}"/>, and a <see cref="ISimulationProcessorPipeline{TState,TArgs,TReturn}"/> is called at the beginning of the simulation, for each successful execution of a <see cref="ISimulationStep"/> as well as when the simulation is finished and when a return value can be provided</description>
///   </item>
/// </list>
/// </summary>
/// <typeparam name="TArgs">A container containing the arguments of the simulated function</typeparam>
/// <typeparam name="TReturn">The return type of the simulated function</typeparam>
/// <typeparam name="TState">The initial state when the simulation begins</typeparam>
/// <inheritdoc />
public interface ISimulationOrchestrator<in TState, in TArgs, TReturn> : ISimulationOrchestrator<TState, TArgs>
{
    new Task<TReturn> ExecuteMethodSimulation(TState initialState, TArgs inputArguments,
        IEnumerable<ISimulationStep> simulationSteps);
}