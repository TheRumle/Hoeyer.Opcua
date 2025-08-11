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
    ValueTask<(EntitySubscription subscription, IReadOnlyList<MonitoredItem> variableMonitoring)> CreateAndMonitorAll(
        IEntitySession session,
        IAgent node,
        Action<MonitoredItem, MonitoredItemNotificationEventArgs> callback,
        CancellationToken cancel = default);

    ValueTask<IReadOnlyList<MonitoredEntityItem>> MonitorAllProperties(EntitySubscription subscription,
        IAgent node, CancellationToken cancel = default);

    ValueTask<EntitySubscription> GetOrCreateSubscriptionWithCallback(
        IEntitySession session,
        string subscriptionName,
        Action<MonitoredItem, MonitoredItemNotificationEventArgs> callback,
        CancellationToken cancel = default);

    ValueTask<IReadOnlyList<MonitoredEntityItem>> MonitorProperties(EntitySubscription subscription,
        IEnumerable<(NodeId Id, string Name)> nodesToMonitor, CancellationToken cancel = default);

    ValueTask<MonitoredEntityItem> MonitorProperty(EntitySubscription subscription,
        (NodeId Id, string Name) nodeToMonitor, CancellationToken cancel = default);
}