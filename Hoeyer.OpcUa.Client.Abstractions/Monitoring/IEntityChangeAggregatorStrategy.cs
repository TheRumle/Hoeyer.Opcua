using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Abstractions.Monitoring;

internal interface IEntityChangeAggregatorStrategy<T>
{
    public void Register(MonitoredItemNotification item);
}