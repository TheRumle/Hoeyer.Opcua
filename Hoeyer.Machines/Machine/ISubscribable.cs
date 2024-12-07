using Hoeyer.Machines.StateSnapshot;

namespace Hoeyer.Machines.Machine;

public interface ISubscribable<TState>
{
    public StateChangeSubscription<TState> Subscribe(IStateChangeSubscriber<TState> stateChangeSubscriber);
}