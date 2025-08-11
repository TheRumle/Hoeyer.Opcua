using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public static class OpcUaEntityServicesLoader
{
    private static readonly Lazy<ImmutableHashSet<EntityServiceInfo>> Services = new(() => OpcUaEntityTypes
        .GenericServices
        .Union(OpcUaEntityTypes.NonGenericServices)
        .Union(OpcUaEntityTypes.BehaviourImplementations)
        .ToImmutableHashSet());

    public static List<OpcUaEntityServiceConfigurationException> AddEntityServices(this IServiceCollection services)
    {
        List<OpcUaEntityServiceConfigurationException> errs = [];
        foreach (var service in Services.Value)
        {
            try
            {
                service.AddToCollection(services);
            }
            catch (OpcUaEntityServiceConfigurationException ex)
            {
                errs.Add(ex);
            }
            catch (Exception e)
            {
                errs.Add(new OpcUaEntityServiceConfigurationException(e.Message));
            }
        }

        return errs;
    }
}