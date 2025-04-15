using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services;

public record GenericEntityServiceTypeInfo(Type ServiceType, Type ImplementationType) : IEntityServiceTypeInfo
{
    public ServiceLifetime ServiceLifetime { get; } = ImplementationType.GetCustomAttribute<OpcUaEntityServiceAttribute>().Lifetime;

    public Type ServiceType { get; } = ServiceType;
    public Type ImplementationType { get; } = ImplementationType;

    /// <inheritdoc />
    public override string ToString() => $"{ServiceType.Name} being implemented by {ImplementationType.Name}";
}