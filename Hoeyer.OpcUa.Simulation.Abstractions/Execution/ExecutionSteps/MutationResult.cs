using System;

namespace Hoeyer.OpcUa.Simulation.Abstractions.Execution.ExecutionSteps;

public sealed record MutationResult<T>(T ReachedState, DateTime TimeOfMutation)
{
    public T ReachedState { get; } = ReachedState;
    public DateTime TimeOfMutation { get; } = TimeOfMutation;
}