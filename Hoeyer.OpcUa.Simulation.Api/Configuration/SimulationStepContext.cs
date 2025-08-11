namespace Hoeyer.OpcUa.Simulation.Api.Configuration;

public sealed class SimulationStepContext<TAgent, TArgsContainer>(TAgent state, TArgsContainer args)
{
    public TAgent State { get; } = state;
    public TArgsContainer Arguments { get; set; } = args;
}