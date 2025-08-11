using System.Collections.Generic;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Api.Configuration;

/// <summary>
/// Used to configure simulated behaviour that occurs when the method, indicated with the <typeparamref name="TArgs"/> argument, is called. 
/// </summary>
/// <typeparam name="TEntity">The current state of the entity that can be used to compute what should happen.</typeparam>
/// <typeparam name="TArgs">A structure representing the input arguments provided when the simulated method was called</typeparam>
public interface ISimulation<TEntity, TArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        ISimulationBuilder<TEntity, TArgs> onMethodCall);
}

public interface ISimulation<TEntity, TArgs, out TReturn>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        ISimulationBuilder<TEntity, TArgs, TReturn> config);
}