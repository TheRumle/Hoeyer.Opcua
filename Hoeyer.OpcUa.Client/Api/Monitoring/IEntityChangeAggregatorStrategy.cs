using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

internal interface IEntityChangeAggregatorStrategy<T>
{
    public void Register(MonitoredItemNotification item);
}