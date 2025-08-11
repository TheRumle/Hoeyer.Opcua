using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core.Api;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface IMonitorItemsFactory<T>
{
    ValueTask<(AgentSubscription subscription, IReadOnlyList<MonitoredItem> variableMonitoring)> CreateAndMonitorAll(
        IAgentSession session,
        IAgent node,
        Action<MonitoredItem, MonitoredItemNotificationEventArgs> callback,
        CancellationToken cancel = default);

    ValueTask<IReadOnlyList<MonitoredAgentItem>> MonitorAllProperties(AgentSubscription subscription,
        IAgent node, CancellationToken cancel = default);

    ValueTask<AgentSubscription> GetOrCreateSubscriptionWithCallback(
        IAgentSession session,
        string subscriptionName,
        Action<MonitoredItem, MonitoredItemNotificationEventArgs> callback,
        CancellationToken cancel = default);

    ValueTask<IReadOnlyList<MonitoredAgentItem>> MonitorProperties(AgentSubscription subscription,
        IEnumerable<(NodeId Id, string Name)> nodesToMonitor, CancellationToken cancel = default);

    ValueTask<MonitoredAgentItem> MonitorProperty(AgentSubscription subscription,
        (NodeId Id, string Name) nodeToMonitor, CancellationToken cancel = default);
}