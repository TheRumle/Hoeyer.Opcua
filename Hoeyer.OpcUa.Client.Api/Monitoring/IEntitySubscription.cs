using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface IEntitySubscription : IDisposable
{
    public uint Id { get; }
    IReadOnlyList<MonitoredEntityItem> EntityItems { get; }
    public void AddEntityItem(MonitoredEntityItem item);
    public void RemoveEntityItem(MonitoredEntityItem item);
    public void AddEntityItems(IEnumerable<MonitoredEntityItem> items);
}