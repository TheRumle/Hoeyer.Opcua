using Hoeyer.OpcUa.Core.Application.RequestResponse;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api.RequestResponse;

public sealed class MonitorItemCreateResponse : StatusCodeResponse<MonitoredItemCreateRequest, IMonitoredItem>
{
    public int OriginalIndex { get; }

    private MonitorItemCreateResponse(int originalIndex, MonitoredItemCreateRequest request,  StatusCode code, string error):base(request, code, error)
    {
        OriginalIndex = originalIndex;
    }
    
    private MonitorItemCreateResponse(int originalIndex, MonitoredItemCreateRequest request, IMonitoredItem item):base(request, () => (item, StatusCodes.Good))
    {
        OriginalIndex = originalIndex;
    }


    public static MonitorItemCreateResponse Ok(int index, MonitoredItemCreateRequest request,  IMonitoredItem item) => new(index, request, item);
    public static MonitorItemCreateResponse NotSupported(int index, MonitoredItemCreateRequest request, string error) => new(index, request, StatusCodes.BadNotSupported, error);


    /// <inheritdoc />
    public override string RequestString()
    {
        string mode = Request.MonitoringMode switch
        {
            MonitoringMode.Disabled => nameof(MonitoringMode.Disabled),
            MonitoringMode.Sampling => nameof(MonitoringMode.Sampling),
            MonitoringMode.Reporting => nameof(MonitoringMode.Reporting),
            _ => "Unspecified"
        };
        
        return $"""
                Mode: {mode},
                Interval: {Request.RequestedParameters.SamplingInterval},
                ItemToMonitor: {Request.ItemToMonitor},
                """;
    }
}