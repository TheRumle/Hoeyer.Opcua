using System.Collections.Generic;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Api.Configuration;

/// <summary>
/// Used to configure simulated behaviour that occurs when the method, indicated with the <typeparamref name="TArgs"/> argument, is called. 
/// </summary>
/// <typeparam name="TAgent">The current state of the agent that can be used to compute what should happen.</typeparam>
/// <typeparam name="TArgs">A structure representing the input arguments provided when the simulated method was called</typeparam>
public interface ISimulation<TAgent, TArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        ISimulationBuilder<TAgent, TArgs> onMethodCall);
}

public interface ISimulation<TAgent, TArgs, out TReturn>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        ISimulationBuilder<TAgent, TArgs, TReturn> config);
}