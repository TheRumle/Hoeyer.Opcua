using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Hoeyer.Common.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services;

public interface IEntityServiceTypeInfo
{
    Type ServiceType { get; }
    Type ImplementationType { get; }
    ServiceLifetime ServiceLifetime { get; }
}

public record GenericEntityServiceTypeInfo : IEntityServiceTypeInfo
{
    public GenericEntityServiceTypeInfo(Type ServiceType, Type ImplementationType)
    {
        this.ServiceType = ServiceType;
        this.ImplementationType = ImplementationType;
        ServiceLifetime = ImplementationType.GetCustomAttribute<OpcUaEntityServiceAttribute>().Lifetime;
    }

    public ServiceLifetime ServiceLifetime { get; set; }

    public Type ServiceType { get; set; }
    public Type ImplementationType { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{ServiceType.Name} being implemented by {ImplementationType.Name}";
}

public record InstantiatedEntityServiceTypeInfo : IEntityServiceTypeInfo
{
    public ServiceLifetime ServiceLifetime { get; }
    public Type Entity { get; set; }
    public Type ServiceType { get; set; }
    public Type ImplementationType { get; set; }
    
    public InstantiatedEntityServiceTypeInfo(Type ServiceType, Type ImplementationType)
    {
        this.ServiceType = ServiceType;
        this.ImplementationType = ImplementationType;
        
        var implementedServiceInterface = ImplementationType
            .GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == ServiceType);
        
        Entity = implementedServiceInterface.GenericTypeArguments.FirstOrDefault()!;
        ServiceLifetime = ImplementationType.GetCustomAttribute<OpcUaEntityServiceAttribute>().Lifetime;
    }

    /// <inheritdoc />

    /// <inheritdoc />
    public override string ToString() => $"{ServiceType.Name} being implemented by {ImplementationType.Name}";
}