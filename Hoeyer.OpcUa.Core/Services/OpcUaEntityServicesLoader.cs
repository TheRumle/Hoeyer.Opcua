using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hoeyer.OpcUa.Core.Reflections;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services;

internal static class OpcUaEntityServicesLoader
{
    internal readonly static FrozenSet<EntityServiceTypeContext> EntityServiceTypeContexts = GetEntityServiceContexts().ToFrozenSet();
    
    public static EntityServiceTypeContext ConstructEntityServiceContext(this IEntityServiceTypeInfo info)
    {
        return info switch
        {
            GenericEntityServiceTypeInfo generic => ConstructEntityServiceContext(generic),
            InstantiatedEntityServiceTypeInfo instantiated => ConstructEntityServiceContext(instantiated),
            _ => throw new ArgumentOutOfRangeException(nameof(info))
        };
    } 
    
    private static EntityServiceTypeContext ConstructEntityServiceContext(
        this GenericEntityServiceTypeInfo genericEntityServiceTypeInfo,
        Type entity)
    {
        var instantiatedServiceImpl = genericEntityServiceTypeInfo.ImplementationType.MakeGenericType(entity);
        return new EntityServiceTypeContext(instantiatedServiceImpl, genericEntityServiceTypeInfo.ServiceType, entity, genericEntityServiceTypeInfo.ServiceLifetime);
    }

    private static EntityServiceTypeContext ConstructEntityServiceContext(
        this InstantiatedEntityServiceTypeInfo typeInfo)
    {
        return new EntityServiceTypeContext(typeInfo.ImplementationType, typeInfo.InstantiatedServiceType, typeInfo.Entity, typeInfo.ServiceLifetime);
    }

    
    internal static IEnumerable<OpcUaEntityServiceConfigurationException> AddEntityServices(IServiceCollection services)
    {
        var missingServicesErrors = AssertAllEntitiesHaveAllServices(EntityServiceTypeContexts).ToList();
        if (missingServicesErrors.Count > 0) return missingServicesErrors;
        
        foreach (var context in EntityServiceTypeContexts)
        {
            context.AddToCollection(services);
        }

        return [];
    }

    private static ImmutableHashSet<EntityServiceTypeContext> GetEntityServiceContexts()
    {
        return OpcUaEntityTypes.Entities
            .SelectMany(entity => OpcUaEntityTypes
                .GenericServices
                .Select(service => service.ConstructEntityServiceContext(entity)))
            .Union(OpcUaEntityTypes
                .InstantiatedServices
                .Select(ConstructEntityServiceContext))
            .ToImmutableHashSet();
    }
    
    private static IEnumerable<OpcUaEntityServiceConfigurationException> AssertAllEntitiesHaveAllServices(IEnumerable<EntityServiceTypeContext> serviceContexts)
    {
        var serviceContextGroups = from serviceContext in serviceContexts
            group serviceContext by serviceContext.ServiceType
            into grouping
            select (ServiceType: grouping.Key, FoundServices: grouping.ToList());

        foreach (var (key, servicesFor) in serviceContextGroups.ToList())
        {
            var entitiesWithoutService = OpcUaEntityTypes.Entities.Except(servicesFor.Select(e => e.Entity));
            foreach (var entity in entitiesWithoutService)
            {
                yield return OpcUaEntityServiceConfigurationException.ServiceNotConfigured(entity, key);
            }
        }
    }
}