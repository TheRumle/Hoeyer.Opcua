using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface IMonitorItemsFactory<T>
{
    ValueTask<(Subscription subscription, IEnumerable<MonitoredItem> variableMonitoring)> GetOrCreate(
        ISession session,
        IEntityNode node,
        CancellationToken cancel);

    (Subscription Subscription, List<MonitoredItem> items) Create(ISession session, IEntityNode node);
}