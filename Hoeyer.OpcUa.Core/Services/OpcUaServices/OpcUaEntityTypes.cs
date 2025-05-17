using System;
using System.Collections.Frozen;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Reflection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public static class OpcUaEntityTypes
{
    public readonly static FrozenSet<Type> TypesFromReferencingAssemblies = typeof(OpcUaEntityAttribute)
        .GetTypesFromConsumingAssemblies()
        .ToFrozenSet();

    public static readonly FrozenSet<Type> Entities = TypesFromReferencingAssemblies
        .Where(type => type.IsAnnotatedWith<OpcUaEntityAttribute>())
        .ToFrozenSet();

    public static readonly FrozenSet<(Type service, Type entity)> EntityBehaviours = TypesFromReferencingAssemblies
        .Select(service => (methodService: service,
            attributeInstance: service.GetAnnotationInstance(typeof(OpcUaEntityMethodsAttribute<>))))
        .Where(annotationInstance => annotationInstance.attributeInstance is not null)
        .Select(e => (e.methodService, e.attributeInstance!.GenericTypeArguments[0]))
        .ToFrozenSet();

    public static readonly FrozenSet<Type> ServiceTypes = TypesFromReferencingAssemblies
        .Where(type => type.IsAnnotatedWith<OpcUaEntityServiceAttribute>())
        .ToFrozenSet();

    public static readonly FrozenSet<GenericEntityServiceTypeInfo> GenericServices = ServiceTypes
        .Where(e => e.IsGenericTypeDefinition)
        .SelectMany(type =>
        {
            return type.GetCustomAttributes<OpcUaEntityServiceAttribute>()
                .Select(attr => new GenericEntityServiceTypeInfo(
                    attr,
                    type));
        })
        .ToFrozenSet();

    public static readonly FrozenSet<InstantiatedEntityServiceTypeInfo> InstantiatedServices = ServiceTypes
        .Where(e => e is { IsGenericTypeDefinition: false, IsInterface: false, IsAbstract: false })
        .SelectMany(type =>
        {
            return type.GetCustomAttributes<OpcUaEntityServiceAttribute>()
                .Select(attr => new InstantiatedEntityServiceTypeInfo(
                    attr,
                    type));
        })
        .ToFrozenSet();

    public static readonly FrozenSet<IEntityServiceTypeInfo> AllServices = GenericServices
        .Select(IEntityServiceTypeInfo (e) => e)
        .Union(InstantiatedServices)
        .ToFrozenSet();
}