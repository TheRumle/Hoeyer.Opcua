using System;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;

namespace Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

public sealed record MutationResult<T>(T PreviousState, T ReachedState, DateTime TimeOfMutation)
{
    public T PreviousState { get; } = PreviousState;
    public T ReachedState { get; } = ReachedState;
    public DateTime TimeOfMutation { get; } = TimeOfMutation;
    public ActionType Action { get; } = ActionType.StateMutation;

    public void Deconstruct(out T previousState, out DateTime timeOfMutation, out T reachedState, out ActionType action)
    {
        previousState = PreviousState;
        reachedState = ReachedState;
        timeOfMutation = TimeOfMutation;
        action = Action;
    }
}