using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Core.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Extensions.Logging;

public class ArgumentInfo(string Name, object Value)
{
    public ArgumentInfo(Argument argument):this(argument.Name, argument.Value)
    {}
    public object Value { get; } = Value;
    public string Name { get; } = Name;
        
    public static ArgumentInfo Of(Argument argument) => new(argument.Name, argument.Value);
}

public static class LoggingExtensions
{

    
    public static object ToLoggingObjectWithArguments(this MethodState method)
    {
        var outArgs = method.OutputArguments?.Value?.Select(ArgumentInfo.Of).ToArray();
        var inArgs = method.InputArguments?.Value?.Select(ArgumentInfo.Of).ToArray();
        
        if (outArgs != null && inArgs != null)
        {
            return new
            {
                Info = method.CoreInfoObject(),
                Arguments = inArgs,
                Result = outArgs
            };
        }

        if (outArgs != null)
        {
            return new
            {
                Info = method.CoreInfoObject(),
                Arguments = inArgs
            };
        }

        if (inArgs != null)
        {
            return new
            {
                Info = method.CoreInfoObject(),
                Arguments = inArgs
            };
        }

        return new
        {
            Info = method.CoreInfoObject(),
            method.AreEventsMonitored,
        };
    }

    public static object ToLoggingObject(this MethodState method)
    {
        return new
        {
            Name = method.BrowseName.Name,
            Details = method.CoreInfoObject(),
            Arguments = method?.InputArguments.Value.Select(ArgumentInfo.Of).ToArray()
        };
    }
    
    public static object ToLoggingObject(this BaseObjectState node) => CoreInfoObject(node);
    
    public static object ToLoggingObject(this IEntityNode node)
    {
        return new
        {
            Entity = node.BaseObject.ToLoggingObject(),
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