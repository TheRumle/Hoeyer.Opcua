using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services;

public record InstantiatedEntityServiceTypeInfo : IEntityServiceTypeInfo
{
    public ServiceLifetime ServiceLifetime { get; }
    public Type Entity { get; set; }
    public Type InstantiatedServiceType { get; set; }
    public Type ImplementationType { get; set; }
    
    public InstantiatedEntityServiceTypeInfo(OpcUaEntityServiceAttribute attr, Type ImplementationType)
    {
        InstantiatedServiceType = attr.ServiceType;
        this.ImplementationType = ImplementationType;
        
        var implementedServiceInterface = ImplementationType
            .GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == InstantiatedServiceType);
        
        Entity = implementedServiceInterface.GenericTypeArguments.FirstOrDefault()!;
        ServiceLifetime = attr.Lifetime;
    }

    /// <inheritdoc />

    /// <inheritdoc />
    public override string ToString() => $"{InstantiatedServiceType.Name} being implemented by {ImplementationType.Name}";
}