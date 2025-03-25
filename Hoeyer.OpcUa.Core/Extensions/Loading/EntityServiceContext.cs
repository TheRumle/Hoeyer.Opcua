using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Extensions.Loading;

/// <summary>
///     Represents any service MyService : IMyService&lt;T&gt; where T is a type annotated with
///     <see cref="OpcUaEntityAttribute" />
/// </summary>
internal readonly record struct EntityServiceContext
{
    public readonly Type ConcreteServiceType;
    public readonly Type Entity;
    public readonly Type ImplementationType;
    public readonly Type ServiceType;


    public EntityServiceContext(Type implementationType, Type serviceType, Type entity)
    {
        Entity = entity;
        if (Entity.GetCustomAttribute<OpcUaEntityAttribute>() == null)
        {
            throw new ArgumentException(
                $"The specified type is not annotated as an OpcUaEntity using the {nameof(OpcUaEntityAttribute)}");
        }

        ImplementationType = implementationType;
        ServiceType = serviceType;

        var implementationImplementsService = Array.Find(
                                                  ImplementationType.GetInterfaces(),
                                                  type => type == serviceType || (type.IsGenericType &&
                                                      type.GetGenericTypeDefinition() == serviceType)) !=
                                              null;

        if (!implementationImplementsService)
        {
            throw new ArgumentException($"{ImplementationType.FullName} does not implement {serviceType}");
        }


        ConcreteServiceType = ServiceType.MakeGenericType(Entity);
        if (!ConcreteServiceType.GetGenericArguments().Contains(entity))
        {
            throw new ArgumentException(
                $"The type {ImplementationType} is not parameterized with {entity} but was instead concrete type of {ConcreteServiceType}");
        }
    }

    public static bool TryCreateFromTypeImplementing(Type type, Type service, out EntityServiceContext context)
    {
        var implementedServiceInterface = type
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == service);

        if (implementedServiceInterface == null)
        {
            context = default;
            return false;
        }

        var entityType = implementedServiceInterface.GenericTypeArguments.FirstOrDefault()!;

        if (entityType.GetCustomAttribute<OpcUaEntityAttribute>() == null)
        {
            throw new OpcUaServiceConfigurationException(
                $"The type '{entityType.FullName}' is not annotated as an OpcUaEntity using the {nameof(OpcUaEntityAttribute)}");
        }

        context = new EntityServiceContext(type, service, entityType);
        return true;
    }


    public ServiceDescriptor ToServiceDescriptor(ServiceLifetime lifetime)
    {
        return new ServiceDescriptor(ConcreteServiceType, ImplementationType, lifetime);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{ServiceType.Name}<{Entity.Name}> being implemented by {ImplementationType.Name}";
    }
}