using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentResults;
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
            session.LastKeepAliveTime,
        };
    }
    
    public static object ToLoggingObject(this Node node)
    {
        return new
        {
            node.NodeId,
            node.BrowseName,
            node.Handle,
            node.NodeClass,
            References = node.References.Select(e => new
            {
                TypeId = new {
                    IdentifierObject = e.TypeId.Identifier,
                    e.ReferenceTypeId,
                },
                e.TargetId
            }),
            Description = node.Description.Text,
        };
    }

    public static object ToLoggingObject(this IEnumerable<BrowseDescription> subscription)
    {
        return subscription.Select(des => new
        {
            des.NodeId,
            des.TypeId,
            des.ResultMask,
            NodeTypeFilter = des.NodeClassMask,
            des.Handle
        });
    }

    public static object ToLoggingObject(this IEnumerable<IError> result)
    {
        return result.Select(e => e.Message.ToString());
    }

    public static object ToLoggingObject(this DiagnosticInfoCollection collection, Predicate<StatusCode>? filter = null)
    {
        var f = filter ?? StatusCode.IsNotGood;
        
        return collection
            .Where(e => f.Invoke(e.InnerStatusCode))
            .Select(diagnostic => diagnostic);
    }
}