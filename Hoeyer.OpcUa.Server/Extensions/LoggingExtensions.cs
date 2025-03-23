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
}