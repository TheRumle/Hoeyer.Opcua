using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public static class OpcUaEntityServicesLoader
{
    internal static List<OpcUaEntityServiceConfigurationException> AddEntityServices(IServiceCollection services)
    {
        ImmutableHashSet<EntityServiceInfo> genericServices = OpcUaEntityTypes.GenericServices
            .Union(OpcUaEntityTypes.NonGenericServices)
            .Union(OpcUaEntityTypes.BehaviourImplementations)
            .Union(GetLoaderServiceContexts()) // loaders are 
            .ToImmutableHashSet();

        foreach (EntityServiceInfo? service in genericServices)
        {
            service.AddToCollection(services);
        }

        return [];
    }

    private static IEnumerable<EntityServiceInfo> GetLoaderServiceContexts()
    {
        Type loaderType = typeof(IEntityLoader<>);
        return OpcUaEntityTypes
            .TypesFromReferencingAssemblies
            .Select(type =>
            {
                Type? foundLoaderInterface = type
                    .GetInterfaces()
                    .FirstOrDefault(@interface => @interface.Namespace == loaderType.Namespace
                                                  && @interface.IsConstructedGenericType &&
                                                  @interface.GetGenericTypeDefinition() == loaderType);

                if (foundLoaderInterface is null) return null!;

                var lifetime = ServiceLifetime.Singleton;
                Type instantiatedService = foundLoaderInterface;
                Type? entity = foundLoaderInterface.GenericTypeArguments[0];
                return new EntityServiceInfo(instantiatedService, type, entity, lifetime);
            })
            .Where(foundType => foundType is not null);
    }
}