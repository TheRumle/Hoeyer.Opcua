using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Core.Application.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Extensions.Logging;

public static class LoggingExtensions
{
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
            }).ToCommaSeparatedString(),
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


    public static object ToLoggingObject(this DiagnosticInfoCollection collection, Predicate<StatusCode>? filter = null)
    {
        var f = filter ?? StatusCode.IsNotGood;
        
        return collection
            .Where(e => f.Invoke(e.InnerStatusCode))
            .Select(diagnostic => diagnostic);
    }

    public static object ToLoggingObject<TRequest, TResponse>(this StatusCodeResponse<TRequest, TResponse> response)
    {
        if (response.IsSuccess)
        {
            return new
            {
                response.Error,
                Request = response.RequestString(),
                ResponseCode = response.ResponseCode.ToString()
            };
        }

        return new
        {
            Request = response.RequestString(),
            Response = response.ResponseCode.ToString()
        };
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
    
}