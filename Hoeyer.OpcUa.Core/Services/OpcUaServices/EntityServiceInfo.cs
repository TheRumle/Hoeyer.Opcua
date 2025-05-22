using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

internal sealed record EntityServiceInfo
{
    /// <summary>
    /// Represents a Service of type IMyService &lt;T&gt; where T remains generic.
    /// </summary>
    public EntityServiceInfo(
        Type serviceType,
        Type ImplementationType,
        Type Entity,
        ServiceLifetime lifetime
    )
    {
        ServiceLifetime = lifetime;
        ServiceType = serviceType;
        this.ImplementationType = ImplementationType;
        this.Entity = Entity;

        if (Entity.GetCustomAttribute<OpcUaEntityAttribute>() is null)
            throw new ArgumentException($"Entity must be annotated with {nameof(OpcUaEntityAttribute)}");
    }

    public ServiceLifetime ServiceLifetime { get; }

    public Type ServiceType { get; }
    public Type ImplementationType { get; }

    public Type Entity { get; }

    public override string ToString() => $"{ServiceType.FullName} being implemented by {ImplementationType.FullName}";

    public IServiceCollection AddToCollection(IServiceCollection serviceCollection)
    {
        serviceCollection.Add(new ServiceDescriptor(ServiceType, ImplementationType, ServiceLifetime));
        serviceCollection.Add(new ServiceDescriptor(ImplementationType, ImplementationType, ServiceLifetime));
        return serviceCollection;
    }
}