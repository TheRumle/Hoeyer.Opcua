using System;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed record ReturnValueStepResult<TAgent, TReturn>(
    TAgent PreviousState,
    TAgent ReachedState,
    TReturn ReturnValue,
    DateTime TimeCreated)
{
    public TAgent PreviousState { get; } = PreviousState;
    public TAgent ReachedState { get; } = ReachedState;
    public TReturn ReturnValue { get; } = ReturnValue;
    public DateTime TimeCreated { get; } = TimeCreated;
}