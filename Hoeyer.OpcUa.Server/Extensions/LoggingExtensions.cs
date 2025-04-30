using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Extensions;

public static class LoggingExtensions
{
    public static object ToLoggingObject(this RequestHeader requestHeader)
    {
        return new
        {
            requestHeader.Timestamp,
            AdditionalHeader = requestHeader.AdditionalHeader.ToString(),
            requestHeader.RequestHandle,
        };
    }

    public static object ToLoggingObject(this ResponseHeader response)
    {
        return new
        {
            response.Timestamp,
            AdditionalHeader = response.AdditionalHeader.ToString(),
            response.RequestHandle,
            response.ServiceDiagnostics,
        };
    }
}