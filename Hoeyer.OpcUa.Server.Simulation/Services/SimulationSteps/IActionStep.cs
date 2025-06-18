using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

public interface IActionStep<in TArgs> : ISimulationStep
{
    internal void Execute(TArgs args);
}

public interface IAsyncActionStep<in TArgs> : ISimulationStep
{
    internal ValueTask Execute(TArgs args);
}