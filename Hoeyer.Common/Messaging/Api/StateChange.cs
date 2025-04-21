namespace Hoeyer.Common.Messaging.Api;

public readonly record struct StateChange<TState, TValue>(TState State, TValue OldValue, TValue NewValue)
{
    public TValue NewValue { get; } = NewValue;
    public TState State { get; } = State;
    public TValue OldValue { get; } = OldValue;
}