using Hoeyer.Common.Extensions;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Extensions;

public static class LoggingExtensions
{
    public static object ToLoggingObject(this ISession session)
    {
        return new
        {
            session.Identity,
            session.IdentityHistory,
            session.SessionName,
            Subscriptions = session.Subscriptions?.Select(e => e.Id),
            session.NamespaceUris,
            session.Handle,
            session.KeepAliveInterval,
            session.LastKeepAliveTime
        };
    }

    public static object ToLoggingObject(this Subscription subscription) =>
        new
        {
            subscription.Id,
            subscription.Priority,
            subscription.DisplayName,
            subscription.PublishingEnabled,
            subscription.PublishTime,
            subscription.LastNotificationTime,
            subscription.TransferId
        };

    public static object ToLoggingObject(this MonitoredItem item) =>
        new
        {
            Subscription = item.Subscription.ToLoggingObject(),
            item.Created,
            TypeBeingMonitored = item.NodeClass.ToString(),
            Status = item.Status.ToString(),
            LastValue = item.LastValue.ToString(),
            item.SamplingInterval,
            Monitored = item.StartNodeId.ToString()
        };

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
            UserIdentityTokens = item.UserIdentityTokens.Select(e => new { e.PolicyId, e.TokenType }.ToString())
                .ToCommaSeparatedString()
        };
    }
}