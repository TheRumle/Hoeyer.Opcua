using System;
using Hoeyer.OpcUa.Simulation.Abstractions.PostProcessing;

namespace Hoeyer.OpcUa.Simulation.Abstractions.Execution;

public record SimulationResult<TState>(
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