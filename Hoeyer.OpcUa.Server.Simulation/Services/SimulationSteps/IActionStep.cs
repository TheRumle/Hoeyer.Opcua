namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

public interface IActionStep<in TArgs> : ISimulationStep
{
    internal void Execute(TArgs args);
}