using System;

namespace Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

public sealed record ReturnValueStepResult<TEntity, TReturn>(
    TEntity PreviousState,
    TEntity ReachedState,
    TReturn ReturnValue,
    DateTime TimeCreated)
{
    public TEntity PreviousState { get; } = PreviousState;
    public TEntity ReachedState { get; } = ReachedState;
    public TReturn ReturnValue { get; } = ReturnValue;
    public DateTime TimeCreated { get; } = TimeCreated;
}