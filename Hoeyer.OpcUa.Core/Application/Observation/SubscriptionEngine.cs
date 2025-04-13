﻿using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Application.Observation;

public class SubscriptionEngine<T>(StateContainer<T> stateContainer)
{
    public List<StateChangeSubscription<T>> Subscriptions { get; } = new();

    public void ClearAll()
    {
        foreach (var var in Subscriptions)
            var.Dispose();
        Subscriptions.Clear();
    }

    public StateChangeSubscription<T> SubscribeToMachine(IStateChangeSubscriber<T> subscriber)
    {
        var a = stateContainer.Subscribe(subscriber);
        Subscriptions.Add(a);
        return a;
    }
}