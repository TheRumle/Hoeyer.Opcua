using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Api.RequestResponse;

public class EntityMonitorDeleteResponse(ServiceResult result, MonitoringFilterResult FilterResult)
{
    public MonitoringFilterResult FilterResult { get; } = FilterResult;
    public ServiceResult result { get; } = result;
}