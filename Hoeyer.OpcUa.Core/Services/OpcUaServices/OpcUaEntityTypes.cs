using System;
using System.Collections.Frozen;
using System.Linq;
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


    internal static readonly FrozenSet<(Type service, Type entity)> EntityBehaviours = TypesFromReferencingAssemblies
        .Select(service => (methodService: service,
            attributeInstance: service.GetAnnotationInstance(typeof(OpcUaEntityMethodsAttribute<>))))
        .Where(annotationInstance => annotationInstance.attributeInstance is not null)
        .Select(e => (e.methodService, e.attributeInstance!.GenericTypeArguments[0]))
        .ToFrozenSet();
};