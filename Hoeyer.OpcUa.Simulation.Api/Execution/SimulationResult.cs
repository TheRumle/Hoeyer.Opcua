using System;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;

namespace Hoeyer.OpcUa.Simulation.Api.Execution;

public readonly record struct SimulationResult<TState>(
    TState Previous,
    DateTime Time,
    TState Reached,
    ActionType Action)
{
    public SimulationResult(TState Previous,
        TState Reached,
        ActionType Action) : this(Previous, DateTime.Now, Reached, Action)
    {
    }

    public ActionType Action { get; } = Action;
    public TState Reached { get; } = Reached;
    public DateTime Time { get; } = Time;
    public TState Previous { get; } = Previous;
}