using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

internal interface IAgentChangeAggregatorStrategy<T>
{
    public void Register(MonitoredItemNotification item);
}