using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

public interface IMonitorItemsFactory<T>
{
    (Subscription subscription, IEnumerable<MonitoredItem> variableMonitoring) GetOrCreate(ISession session, IEntityNode node);
    (Subscription Subscription, List<MonitoredItem> items) Create(ISession session, IEntityNode node);
}