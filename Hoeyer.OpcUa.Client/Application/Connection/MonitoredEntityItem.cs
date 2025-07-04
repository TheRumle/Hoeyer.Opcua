using System;
using System.Collections.Generic;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Connection;

public sealed class MonitoredEntityItem(MonitoredItem inner) : MonitoredItem(inner), IDisposable
{
    private readonly List<MonitoredItemNotificationEventHandler> _subscribers = new();

    public IReadOnlyList<MonitoredItemNotificationEventHandler> Subscribers => _subscribers.AsReadOnly();

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var subscriber in _subscribers.ToArray())
        {
            Notification -= subscriber;
        }
    }

    // Hide base Notification event with `new` keyword
    public new event MonitoredItemNotificationEventHandler Notification
    {
        add
        {
            base.Notification += value;
            _subscribers.Add(value);
        }
        remove
        {
            base.Notification -= value;
            _subscribers.Remove(value);
        }
    }
}