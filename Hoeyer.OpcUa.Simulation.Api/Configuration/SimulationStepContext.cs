namespace Hoeyer.OpcUa.Simulation.Api.Configuration;

public sealed class SimulationStepContext<TEntity, TArgsContainer>(TEntity state, TArgsContainer args)
{
    public TEntity State { get; } = state;
    public TArgsContainer Arguments { get; set; } = args;
}