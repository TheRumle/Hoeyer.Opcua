using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public static class OpcUaAgentServicesLoader
{
    private static readonly Lazy<ImmutableHashSet<AgentServiceInfo>> Services = new(() => OpcUaAgentTypes
        .GenericServices
        .Union(OpcUaAgentTypes.NonGenericServices)
        .Union(OpcUaAgentTypes.BehaviourImplementations)
        .ToImmutableHashSet());

    public static List<OpcUaAgentServiceConfigurationException> AddAgentServices(this IServiceCollection services)
    {
        List<OpcUaAgentServiceConfigurationException> errs = [];
        foreach (var service in Services.Value)
        {
            try
            {
                service.AddToCollection(services);
            }
            catch (OpcUaAgentServiceConfigurationException ex)
            {
                errs.Add(ex);
            }
            catch (Exception e)
            {
                errs.Add(new OpcUaAgentServiceConfigurationException(e.Message));
            }
        }

        return errs;
    }
}