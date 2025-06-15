using System;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

public sealed record ReturnValueStepResult<T, TReturn>(
    T PreviousState,
    T ReachedState,
    TReturn ReturnValue,
    DateTime TimeCreated)
{
    public T PreviousState { get; } = PreviousState;
    public T ReachedState { get; } = ReachedState;
    public TReturn ReturnValue { get; } = ReturnValue;
    public DateTime TimeCreated { get; } = TimeCreated;
}