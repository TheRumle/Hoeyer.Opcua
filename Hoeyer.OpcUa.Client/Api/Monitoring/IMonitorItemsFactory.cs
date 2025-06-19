using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Core.Api;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface IMonitorItemsFactory<T>
{
    ValueTask<(EntitySubscription subscription, IEnumerable<MonitoredItem> variableMonitoring)> GetOrCreate(
        IEntitySession session,
        IEntityNode node,
        CancellationToken cancel);

    (EntitySubscription Subscription, List<MonitoredEntityItem> items) Create(IEntitySession session, IEntityNode node);
}