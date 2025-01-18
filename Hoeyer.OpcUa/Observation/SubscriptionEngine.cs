using System.Collections.Generic;
using Hoeyer.OpcUa.Entity.State;

namespace Hoeyer.OpcUa.Observation;

public class SubscriptionEngine<T>(StateContainer<T> stateContainer)
{
    public void ClearAll()
    {
        foreach (var var in Subscriptions)
            var.Dispose();
        Subscriptions.Clear();
    }
    
    public List<StateChangeSubscription<T>> Subscriptions { get; } = new();
    public StateChangeSubscription<T> SubscribeToMachine(IStateChangeSubscriber<T> subscriber)
    {
        var a = stateContainer.Subscribe(subscriber);
        Subscriptions.Add(a);
        return a;
    }
    
}