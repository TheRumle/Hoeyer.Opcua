using System;
using System.Linq;
using Hoeyer.Common.Reflection;

namespace Hoeyer.OpcUa.Core.Services;

public interface IEntityServiceTypeInfo
{
    Type ServiceType { get; set; }
    Type ImplementationType { get; set; }
}

public record GenericEntityServiceTypeInfo : IEntityServiceTypeInfo
{
    public GenericEntityServiceTypeInfo(Type ServiceType, Type ImplementationType)
    {
        this.ServiceType = ServiceType;
        this.ImplementationType = ImplementationType;
    }

    public Type ServiceType { get; set; }
    public Type ImplementationType { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{ServiceType.Name} being implemented by {ImplementationType.Name}";
}

public record InstantiatedEntityServiceTypeInfo : IEntityServiceTypeInfo
{
    public InstantiatedEntityServiceTypeInfo(Type ServiceType, Type ImplementationType)
    {
        this.ServiceType = ServiceType;
        this.ImplementationType = ImplementationType;
        
        var implementedServiceInterface = ImplementationType
            .GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == ServiceType);
        
        Entity = implementedServiceInterface.GenericTypeArguments.FirstOrDefault()!;
    }

    public Type Entity { get; set; }
    public Type ServiceType { get; set; }
    public Type ImplementationType { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{ServiceType.Name} being implemented by {ImplementationType.Name}";
}