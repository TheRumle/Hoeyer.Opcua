using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Extensions;

public static class LoggingExtensions
{
    public static object ToLoggingObject(this RequestHeader requestHeader) =>
        new
        {
            requestHeader.Timestamp,
            AdditionalHeader = requestHeader.AdditionalHeader.ToString(),
            requestHeader.RequestHandle,
            requestHeader.AuditEntryId
        };

    public static object ToLoggingObject(this ResponseHeader response) =>
        new
        {
            response.Timestamp,
            AdditionalHeader = response.AdditionalHeader.ToString(),
            response.RequestHandle,
            response.ServiceDiagnostics
        };
}