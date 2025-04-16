using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Reflections;

/// <summary>
///     Represents any service MyService : IMyService&lt;T&gt; where T is a type annotated with
///     <see cref="OpcUaEntityAttribute" />
/// </summary>
internal readonly record struct EntityServiceTypeContext
{
    /// <summary>
    /// Must be private to ensure that <see cref="EntityServiceTypeContext.AddToCollection"/> is used to add to services.
    /// </summary>
    private readonly Type _implementationType; 
    private readonly ServiceLifetime _lifetime;
    public readonly Type ConcreteServiceType;
    public readonly Type Entity;
    public readonly Type ServiceType;


    public EntityServiceTypeContext(Type implementationType, Type serviceType, Type entity, ServiceLifetime lifetime)
    {
        _implementationType = implementationType;
        _lifetime = lifetime;
        ServiceType = serviceType;
        Entity = entity;
        ConcreteServiceType = ServiceType.IsGenericTypeDefinition ? ServiceType.MakeGenericType(Entity) : ServiceType;
        
        if (Entity.GetCustomAttribute<OpcUaEntityAttribute>() == null)
        {
            throw new ArgumentException(
                $"The specified type is not annotated as an OpcUaEntity using the {nameof(OpcUaEntityAttribute)}");
        }

        var implementationImplementsService = Array.Find(
                                                  _implementationType.GetInterfaces(),
                                                  type => type == serviceType || (type.IsGenericType &&
                                                      type.GetGenericTypeDefinition() == serviceType)) !=
                                              null;

        if (!implementationImplementsService)
        {
            throw new ArgumentException($"{_implementationType.FullName} does not implement {serviceType}");
        }

    }

    public IServiceCollection AddToCollection(IServiceCollection serviceCollection)
    {
        serviceCollection.Add(new ServiceDescriptor(ConcreteServiceType, _implementationType, _lifetime));
        serviceCollection.Add(new ServiceDescriptor(_implementationType, _implementationType, _lifetime));
        return serviceCollection;
    }
    

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{ServiceType.Name}<{Entity.Name}> being implemented by {_implementationType.Name}";
    }
}