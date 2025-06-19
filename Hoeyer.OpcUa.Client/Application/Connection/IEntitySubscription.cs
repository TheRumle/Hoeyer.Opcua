using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Client.Application.Connection;

public interface IEntitySubscription : IDisposable
{
    IReadOnlyList<MonitoredEntityItem> EntityItems { get; }
    public void AddEntityItem(MonitoredEntityItem item);
    public void RemoveEntityItem(MonitoredEntityItem item);
    public void AddEntityItems(IEnumerable<MonitoredEntityItem> items);
}