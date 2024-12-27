using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.StateSnapshot;

namespace Hoeyer.OpcUa.Observation;

public class SubscriptionEngine<T>(Machine<T> machine)
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
        var a = machine.Subscribe(subscriber);
        Subscriptions.Add(a);
        return a;
    }
    
}