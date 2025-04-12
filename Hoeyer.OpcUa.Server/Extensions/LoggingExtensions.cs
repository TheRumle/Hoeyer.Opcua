using System.Text.Json;
using Hoeyer.Common.Extensions.LoggingExtensions;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Extensions;

public static class LoggingExtensions
{
    public static IScopeSelected WithSessionContextScope(this ILogLevelSelected logger, IOperationContext context,
        [StructuredMessageTemplate] string scope, params object[] messageArguments)
    {
        var identity = JsonSerializer.Serialize(new
        {
            Session = context.SessionId.ToString(),
            UserIdentity = context.UserIdentity.DisplayName
        }, new JsonSerializerOptions { WriteIndented = true });

        // Prepend a prefix to the message without breaking structured formatting
        var prefixedMessage = $"[{identity}] {scope}";
        return logger.WithScope(prefixedMessage, messageArguments);
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