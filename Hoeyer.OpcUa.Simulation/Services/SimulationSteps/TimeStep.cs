using System;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

internal sealed record TimeStep<TEntity>(TEntity State, TimeSpan TimeSpan, DateTime CreationTime) : ISimulationStep
{
    public TEntity State { get; } = State;
    public TimeSpan TimeSpan { get; } = TimeSpan;
    public DateTime CreationTime { get; } = CreationTime;

    public async Task Execute() => await Task.Delay(TimeSpan);
}