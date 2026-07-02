using System;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed record MutationResult<T>(T ReachedState, DateTime TimeOfMutation)
{
    public T ReachedState { get; } = ReachedState;
    public DateTime TimeOfMutation { get; } = TimeOfMutation;
}