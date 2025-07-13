using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

internal interface IEntityChangeAggregatorStrategy<T>
{
    public void Register(MonitoredItemNotification item);
}