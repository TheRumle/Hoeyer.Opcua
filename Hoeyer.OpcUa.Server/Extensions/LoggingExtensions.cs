using Hoeyer.Common.Extensions.LoggingExtensions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Extensions;

public static class LoggingExtensions
{
    public static IScopeSelected WithSessionContextScope(this ILogLevelSelected logger, IOperationContext context,
        string scopeName)
    {
        return logger.WithScope(scopeName + ": {@Context}", context.ToLoggingObject());
    }

    public static object ToLoggingObject(this RequestHeader requestHeader)
    {
        return new
        {
            requestHeader.Timestamp,
            AdditionalHeader = requestHeader.AdditionalHeader.ToString(),
            requestHeader.RequestHandle,
        };
    }
    
    public static object ToLoggingObject(this IOperationContext context)
    {
        return new
        {
            context.SessionId,
            User = context.UserIdentity.DisplayName,
            context.AuditEntryId
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

    public static object ToLoggingObject(this EndpointDescription description)
    {
        return new
        {
            Url = description.EndpointUrl,
            description.Server.ApplicationUri,
            description.Server.ApplicationName,
        };
    }

    public static object ToLoggingObject(this IEndpointIncomingRequest request)
    {
        return new
        {
            Header = request.Request.RequestHeader.ToLoggingObject(),
            RequestHeader = new
            {
                request.SecureChannelContext.SecureChannelId,
                Endpoint = request.SecureChannelContext.EndpointDescription.ToLoggingObject(),
                CallDataPayload = request.Calldata
            },
        };
    }
}