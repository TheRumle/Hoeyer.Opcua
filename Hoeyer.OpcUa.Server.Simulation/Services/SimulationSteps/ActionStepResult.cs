using System;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

public sealed record ActionStepResult<T>(T PreviousState, T ReachedState, DateTime TimeCreated)
{
    public T PreviousState { get; } = PreviousState;
    public T ReachedState { get; } = ReachedState;
    public DateTime TimeCreated { get; } = TimeCreated;
}