using System.Linq;
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
            session.LastKeepAliveTime,
        };
    }
}