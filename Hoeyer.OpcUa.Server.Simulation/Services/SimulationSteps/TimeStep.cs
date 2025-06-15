using System;
using System.Threading;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

internal sealed record TimeStep<TEntity> : ITimeStep
{
    private readonly object _lock;

    public TimeStep(TEntity State, TimeSpan TimeSpan, object Lock)
    {
        this.State = State;
        this.TimeSpan = TimeSpan;
        _lock = Lock;
    }

    public TEntity State { get; }
    public TimeSpan TimeSpan { get; }

    public void Execute()
    {
        lock (_lock)
        {
            Thread.Sleep(TimeSpan);
        }
    }
}