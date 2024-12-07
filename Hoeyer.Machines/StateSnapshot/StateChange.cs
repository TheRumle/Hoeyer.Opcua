﻿using System;

namespace Hoeyer.Machines.StateSnapshot;

public record StateChange<TState>(TState PreviousState, TState ReachedState, DateTime EnteredStateOn)
{
    public TState PreviousState { get; } = PreviousState;
    public TState ReachedState { get; } = ReachedState;
    public DateTime EnteredStateOn { get; } = EnteredStateOn;
}