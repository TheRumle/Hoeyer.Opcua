using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

/// <summary>
/// Used to configure simulated behaviour that occurs when the method, indicated with the <typeparamref name="TArgs"/> argument, is called. 
/// </summary>
/// <typeparam name="TEntity">The current state of the entity that can be used to compute what should happen.</typeparam>
/// <typeparam name="TArgs">A structure representing the input arguments provided when the simulated method was called</typeparam>
/// NOTE: <typeparamref name="TEntity"/> will be a shared state across simulations and will be locked while the simulation occurs. 
public interface IActionSimulationConfigurator<TEntity, TArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IActionSimulationBuilder<TEntity, TArgs> onMethodCall);
}