using System;
using System.Linq;
using System.Reflection;

namespace Hoeyer.OpcUa.Core.Reflections;

/// <summary>
///     Represents any service MyService : IMyService&lt;T&gt; where T is a type annotated with
///     <see cref="OpcUaEntityAttribute" />
/// </summary>
public readonly record struct EntityServiceTypeContext
{
    public readonly Type ConcreteServiceType;
    public readonly Type Entity;
    public readonly Type ImplementationType;
    public readonly Type ServiceType;


    public EntityServiceTypeContext(Type implementationType, Type serviceType, Type entity)
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

    public static bool TryCreateFromTypeImplementing(Type type, Type service, out EntityServiceTypeContext typeContext)
    {
        var implementedServiceInterface = type
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == service);

        if (implementedServiceInterface == null)
        {
            typeContext = default;
            return false;
        }

        var entityType = implementedServiceInterface.GenericTypeArguments.FirstOrDefault()!;

        if (entityType.GetCustomAttribute<OpcUaEntityAttribute>() == null)
        {
            throw new OpcUaEntityServiceConfigurationException(
                $"The type '{entityType.FullName}' is not annotated as an OpcUaEntity using the {nameof(OpcUaEntityAttribute)}");
        }

        typeContext = new EntityServiceTypeContext(type, service, entityType);
        return true;
    }




    /// <inheritdoc />
    public override string ToString()
    {
        return $"{ServiceType.Name}<{Entity.Name}> being implemented by {ImplementationType.Name}";
    }
}