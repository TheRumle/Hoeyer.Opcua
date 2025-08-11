using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Api.Configuration;

public interface
    ISimulationBuilder<TAgent, TArguments>
    : IComposedSimulationBuilder<TAgent, TArguments, ISimulationBuilder<TAgent, TArguments>>
{
    IEnumerable<ISimulationStep> Build();
}

public interface ISimulationBuilder<TAgent, TArguments, in TReturn>
    : IComposedSimulationBuilder<TAgent, TArguments, ISimulationBuilder<TAgent, TArguments, TReturn>>
{
    /// <summary>
    /// Finalize the simulation configuration by providing an expression used to compute the result of the method call
    /// </summary>
    /// <param name="returnValueFactory">the function expression used to compute the return value.</param>
    /// <typeparam name="TReturn">The return value</typeparam>
    /// <returns>a sequence of ISimulationSteps that has been configured using the builder with the last element being a step containing the return value.</returns>
    IEnumerable<ISimulationStep> WithReturnValue(
        Func<SimulationStepContext<TAgent, TArguments>, TReturn> returnValueFactory);
}