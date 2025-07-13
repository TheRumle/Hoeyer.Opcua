using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public static class OpcUaEntityServicesLoader
{
    public static List<OpcUaEntityServiceConfigurationException> AddEntityServices(this IServiceCollection services)
    {
        ImmutableHashSet<EntityServiceInfo> genericServices = OpcUaEntityTypes.GenericServices
            .Union(OpcUaEntityTypes.NonGenericServices)
            .Union(OpcUaEntityTypes.BehaviourImplementations)
            .ToImmutableHashSet();

        foreach (EntityServiceInfo? service in genericServices)
        {
            service.AddToCollection(services);
        }

        return [];
    }
}