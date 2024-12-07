
using System;

namespace Hoeyer.Machines.StateSnapshot;

public interface  IStateChangeSubscriber<TState> : IDisposable
{
    public void OnStateChange(StateChange<TState> stateChange);
}