using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Connection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public sealed class EntitySubscription : Subscription, IEntitySubscription
{
    private readonly List<MonitoredEntityItem> _entityItems = new();
    public readonly MonitoredItemNotificationEventHandler Callback;

    public EntitySubscription(IEntitySession session,
        Action<MonitoredItem, MonitoredItemNotificationEventArgs> callback) : base(session.Session.DefaultSubscription)
    {
        Callback = new MonitoredItemNotificationEventHandler(callback);
    }

    public EntitySubscription(IEntitySession session, MonitoredItemNotificationEventHandler callback) : base(
        session.Session.DefaultSubscription)
    {
        Callback = callback;
    }

    public IReadOnlyList<MonitoredEntityItem> EntityItems => _entityItems.AsReadOnly();

    public void AddEntityItem(MonitoredEntityItem item)
    {
        AddItem(item);
        _entityItems.Add(item);
        item.Notification += Callback;
    }

    public void AddEntityItems(IEnumerable<MonitoredEntityItem> items)
    {
        foreach (MonitoredEntityItem? item in items)
        {
            AddEntityItem(item);
        }
    }

    public void RemoveEntityItem(MonitoredEntityItem item)
    {
        RemoveItem(item);
        _entityItems.Remove(item);
        item.Notification -= Callback;
    }

    public async Task AttachMonitoredItems(IEnumerable<MonitoredItem> monitoredItem)
    {
        AddItems(monitoredItem);
        await ApplyChangesAsync();
    }

    public async Task DetachMonitoredItems(IEnumerable<MonitoredItem> monitoredItem)
    {
        RemoveItems(monitoredItem);
        await ApplyChangesAsync();
    }
}