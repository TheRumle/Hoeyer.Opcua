using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Abstractions.Services;

namespace Hoeyer.OpcUa.Simulation.Abstractions.Execution.ExecutionSteps;

public sealed record TimeStep(TimeSpan TimeSpan, DateTime CreationTime) : ISimulationStep
{
    public TimeSpan TimeSpan { get; } = TimeSpan;
    public DateTime CreationTime { get; } = CreationTime;

    public async Task Execute(ITimeScaler scaler)
    {
        await Task.Delay(scaler.Scale(TimeSpan));
    }
}