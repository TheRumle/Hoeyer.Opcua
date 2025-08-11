using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Extensions.Logging;

public static class LoggingExtensions
{
    public static object ToLoggingObject(this MethodState method, IList<object> inputArguments)
    {
        Func<object?, string> objToString = (object? value) =>
        {
            if (value is null)
            {
                return "'null'";
            }

            return value is IEnumerable c and not string
                ? $"[\n" + string.Join(",\n ", c)
                : value.ToString();
        };


        var args = $"[\n" +
                   string.Join(",\n", inputArguments.Select(objToString).Select(e => e.ToString()).ToArray())
                   + "\n]";
        return new
        {
            method.BrowseName.Name,
            Details = method.CoreInfoObject(),
            Arguments = args
        };
    }

    public static object ToLoggingObject(this MethodState method)
    {
        return new
        {
            method.BrowseName.Name,
            Arguments = method?.InputArguments.Value.Select(ArgumentInfo.Of).ToArray()
        };
    }

    public static object ToLoggingObject(this BaseObjectState node) => CoreInfoObject(node);

    public static object ToLoggingObject(this IAgent node)
    {
        return new
        {
            Agent = node.BaseObject.ToLoggingObject(),
            Properties = node.PropertyStates.Select(e => e.CoreInfoObject()).ToArray(),
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


    public static object ToLoggingObject(this ApplicationConfiguration configuration)
    {
        return new
        {
            configuration.ApplicationName,
            configuration.ProductUri,
            configuration.Properties,
            Extensions = configuration.ExtensionObjects,
            Other = new
            {
                DomainNames = configuration.GetServerDomainNames(),
            },
            Security = new
            {
                configuration.SecurityConfiguration?.SupportedSecurityPolicies
            },
            KnownDiscoveryUrls = configuration.ClientConfiguration?.WellKnownDiscoveryUrls?.Select(e => e).ToArray()
        };
    }

    private static Object CoreInfoObject(this NodeState node)
    {
        return new
        {
            node.NodeId,
            node.BrowseName,
            node.Handle,
            node.NodeClass,
            Description = node.Description?.Text,
        };
    }
}