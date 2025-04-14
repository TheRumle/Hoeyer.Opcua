using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Reflections;

/// <summary>
///     Represents any service MyService : IMyService&lt;T&gt; where T is a type annotated with
///     <see cref="OpcUaEntityAttribute" />
/// </summary>
public readonly record struct EntityServiceTypeContext
{
    private readonly ServiceLifetime Lifetime;
    public readonly Type ConcreteServiceType;
    public readonly Type Entity;
    public readonly Type ImplementationType;
    public readonly Type ServiceType;


    public EntityServiceTypeContext(Type implementationType, Type serviceType, Type entity, ServiceLifetime lifetime)
    {
        Entity = entity;
        Lifetime = lifetime;
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

        ConcreteServiceType = ServiceType.IsGenericTypeDefinition ? ServiceType.MakeGenericType(Entity) : ServiceType;
    }

    public IServiceCollection AddToCollection(IServiceCollection serviceCollection)
    {
        serviceCollection.Add(new ServiceDescriptor(ConcreteServiceType, ImplementationType, Lifetime));
        serviceCollection.Add(new ServiceDescriptor(ImplementationType, ImplementationType, Lifetime));
        return serviceCollection;
    }
    

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{ServiceType.Name}<{Entity.Name}> being implemented by {ImplementationType.Name}";
    }
}