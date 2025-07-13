using System.Collections.Generic;
using Hoeyer.OpcUa.Client.Api.Connection;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public sealed class EntitySubscription : Subscription, IEntitySubscription
{
    private readonly List<MonitoredEntityItem> _entityItems = new();

    public EntitySubscription(IEntitySession session) : base(session.Session.DefaultSubscription)
    {
    }

    public EntitySubscription(EntitySubscription subscription) : base(subscription)
    {
    }

    public IReadOnlyList<MonitoredEntityItem> EntityItems => _entityItems.AsReadOnly();

    public void AddEntityItem(MonitoredEntityItem item)
    {
        AddItem(item);
        _entityItems.Add(item);
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
    }
}