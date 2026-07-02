using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Simulation.Api.Services;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed record TimeStep(TimeSpan TimeSpan, DateTime CreationTime) : ISimulationStep
{
    public TimeSpan TimeSpan { get; } = TimeSpan;
    public DateTime CreationTime { get; } = CreationTime;

    public async Task Execute(ITimeScaler scaler)
    {
        await Task.Delay(scaler.Scale(TimeSpan));
    }
}