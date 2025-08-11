using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface IAgentSubscription : IDisposable
{
    public uint Id { get; }
    IReadOnlyList<MonitoredAgentItem> AgentItems { get; }
    public void AddAgentItem(MonitoredAgentItem item);
    public void RemoveAgentItem(MonitoredAgentItem item);
    public void AddAgentItems(IEnumerable<MonitoredAgentItem> items);
}