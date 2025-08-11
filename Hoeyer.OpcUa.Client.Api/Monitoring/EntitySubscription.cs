using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Connection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public sealed class AgentSubscription : Subscription, IAgentSubscription
{
    private readonly List<MonitoredAgentItem> _agentItems = new();
    public readonly MonitoredItemNotificationEventHandler Callback;

    public AgentSubscription(IAgentSession session,
        Action<MonitoredItem, MonitoredItemNotificationEventArgs> callback) : base(session.Session.DefaultSubscription)
    {
        Callback = new MonitoredItemNotificationEventHandler(callback);
    }

    public AgentSubscription(IAgentSession session, MonitoredItemNotificationEventHandler callback) : base(
        session.Session.DefaultSubscription)
    {
        Callback = callback;
    }

    public IReadOnlyList<MonitoredAgentItem> AgentItems => _agentItems.AsReadOnly();

    public void AddAgentItem(MonitoredAgentItem item)
    {
        AddItem(item);
        _agentItems.Add(item);
        item.Notification += Callback;
    }

    public void AddAgentItems(IEnumerable<MonitoredAgentItem> items)
    {
        foreach (MonitoredAgentItem? item in items)
        {
            AddAgentItem(item);
        }
    }

    public void RemoveAgentItem(MonitoredAgentItem item)
    {
        RemoveItem(item);
        _agentItems.Remove(item);
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