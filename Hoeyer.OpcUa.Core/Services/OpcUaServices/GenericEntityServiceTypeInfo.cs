using System;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public record GenericEntityServiceTypeInfo(
    OpcUaEntityServiceAttribute Attribute,
    Type ImplementationType) : IEntityServiceTypeInfo
{
    public ServiceLifetime ServiceLifetime { get; } = Attribute.Lifetime;

    public Type ServiceType { get; } = Attribute.ServiceType;
    public Type ImplementationType { get; } = ImplementationType;
    public OpcUaEntityServiceAttribute Attribute { get; } = Attribute;

    /// <inheritdoc />
    public override string ToString() => $"{ServiceType.Name} being implemented by {ImplementationType.Name}";
}