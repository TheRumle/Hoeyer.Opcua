using System;
using System.Collections.Frozen;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Reflection;

namespace Hoeyer.OpcUa.Core.Services;

public static class OpcUaEntityTypes
{
    public readonly static FrozenSet<Type> TypesFromReferencingAssemblies = typeof(OpcUaEntityAttribute)
        .GetTypesFromConsumingAssemblies()
        .ToFrozenSet();
    
    public static readonly FrozenSet<Type> Entities = TypesFromReferencingAssemblies
        .Where(type => type.IsAnnotatedWith<OpcUaEntityAttribute>())
        .ToFrozenSet();

    public static readonly FrozenSet<Type> ServiceTypes = TypesFromReferencingAssemblies
        .Where(type => type.IsAnnotatedWith<OpcUaEntityServiceAttribute>())
        .ToFrozenSet();
    
    public static readonly FrozenSet<GenericEntityServiceTypeInfo> GenericServices = ServiceTypes
        .Where(e=>e.IsGenericTypeDefinition)
        .Select(type => new GenericEntityServiceTypeInfo(
            type.GetCustomAttribute<OpcUaEntityServiceAttribute>().ServiceType,
            type))
        .ToFrozenSet();
    
    public static readonly FrozenSet<InstantiatedEntityServiceTypeInfo> InstantiatedServices = ServiceTypes
        .Where(e=> e is { IsGenericTypeDefinition: false, IsInterface: false, IsAbstract: false })
        .Select(type => new InstantiatedEntityServiceTypeInfo(
            type.GetCustomAttribute<OpcUaEntityServiceAttribute>().ServiceType,
            type))
        .ToFrozenSet();
    
    public static readonly FrozenSet<IEntityServiceTypeInfo> AllServices = GenericServices
        .Select(IEntityServiceTypeInfo (e) => e)
        .Union(InstantiatedServices)
        .ToFrozenSet();
}