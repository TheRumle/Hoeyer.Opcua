using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Extensions;

public static class LoggingExtensions
{

    public static object ToLoggingObject(this IEnumerable<IError> result)
    {
        return result.Select(e => e.Message.ToString());
    }
    
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