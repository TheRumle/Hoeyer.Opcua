using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hoeyer.OpcUa.Core.Extensions.Loading;

internal static class EntityServiceTypeExtensions
{
    public static IEnumerable<EntityServiceContext> GetEntityServicesOfType(this IEnumerable<Type> assemblyTypes,
        Type service)
    {
        if (!service.IsInterface || !service.IsGenericTypeDefinition || service.GetGenericArguments().Length > 1)
            throw new OpcUaConfigurationException(
                "The specified type does not represent a Generic Type Definition. The service type must take 1 generic argument, must be an interface, and must not be a type representing an instantiation of the generic type definition.");

        foreach (var type in assemblyTypes.Where(t => t.IsClass && !t.IsAbstract))
        {
            var context = CreateEntityServiceContext(type, service);
            if (context == null) continue;
            yield return context.Value;
        }
    }

    private static EntityServiceContext? CreateEntityServiceContext(this Type type, Type service)
    {
        var implementedServiceInterface = type
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == service);

        if (implementedServiceInterface == null) return null;
        var entityType = implementedServiceInterface.GenericTypeArguments.FirstOrDefault()!;

        if (entityType.GetCustomAttribute<OpcUaEntityAttribute>() == null)
            throw new OpcUaConfigurationException(
                $"The type '{entityType.FullName}' is not annotated as an OpcUaEntity using the {nameof(OpcUaEntityAttribute)}");

        return new EntityServiceContext(type, service, entityType);
    }
}