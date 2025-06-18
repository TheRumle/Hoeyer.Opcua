using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public interface IFunctionSimulationBuilder<TEntity, TArguments, in TReturn>
{
    /// <summary>
    /// Finalize the simulation configuration by providing an expression used to compute the result of the method call
    /// </summary>
    /// <param name="returnValueFactory">the function expression used to compute the return value.</param>
    /// <typeparam name="TReturn">The return value</typeparam>
    /// <returns>a sequence of ISimulationSteps that has been configured using the builder with the last element being a step containing the return value.</returns>
    IEnumerable<ISimulationStep> WithReturnValue(
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueFactory);

    IFunctionSimulationBuilder<TEntity, TArguments, TReturn> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange);

    IFunctionSimulationBuilder<TEntity, TArguments, TReturn> ChangeStateAsync(
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange);

    IFunctionSimulationBuilder<TEntity, TArguments, TReturn> SideEffect(Action<TArguments> sideEffect);
}