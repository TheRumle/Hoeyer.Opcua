namespace Hoeyer.OpcUa.Simulation.Abstractions.Configuration;

public sealed class SimulationStepContext<TEntity, TArgsContainer>(TEntity state, TArgsContainer args)
{
    public TEntity State { get; } = state;
    public TArgsContainer Arguments { get; set; } = args;
}