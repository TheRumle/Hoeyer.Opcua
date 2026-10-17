using Hoeyer.Common.Extensions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Extensions;

public static class LoggingExtensions
{
    public static object ToLoggingObject(this EndpointDescription? item)
    {
        if (item == null)
        {
            return "[null]";
        }

        return new
        {
            item.EndpointUrl,
            item.SecurityMode,
            item.SecurityPolicyUri,
            UserIdentityTokens = item.UserIdentityTokens.ToArray()?.Select(e => new { e.PolicyId, e.TokenType }.ToString())
                .ToCommaSeparatedString()??"No identity tokens"
        };
    }
}