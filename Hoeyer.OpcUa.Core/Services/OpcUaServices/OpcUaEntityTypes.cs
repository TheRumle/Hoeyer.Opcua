using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Reflection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public static class OpcUaEntityTypes
{
    public static readonly IEnumerable<Type> TypesFromReferencingAssemblies = typeof(OpcUaEntityAttribute)
                                                                                  .GetTypesFromConsumingAssemblies()
                                                                              ?? throw new InvalidOperationException(
                                                                                  "GetTypesFromConsumingAssemblies returned null");

    public static readonly FrozenSet<Type> Entities = TypesFromReferencingAssemblies
        .Where(type => type.IsAnnotatedWith<OpcUaEntityAttribute>())
        .ToFrozenSet();


    public static readonly FrozenSet<(Type ServiceInterface, Type ClientImplementation, Type Entity)> EntityBehaviours
        = TypesFromReferencingAssemblies
            .SelectMany(impl => impl
                .GetInterfaces()
                .Select(iFace => (service: iFace, attribute: GetOpcUaEntityMethodAttributeData(iFace)))
                .Where(tuple => tuple.attribute != null)
                .Select(serviceAttributeTuple => (serviceAttributeTuple.service, impl,
                    serviceAttributeTuple.attribute!.GetType().GenericTypeArguments[0])))
            .ToFrozenSet();

    public static IEnumerable<Type> TypesFromReferencingAssembliesUsingMarker(Type marke) =>
        typeof(OpcUaEntityAttribute)
            .GetTypesFromConsumingAssemblies()
            .Union(marke.GetTypesFromConsumingAssemblies());

    private static Attribute? GetOpcUaEntityMethodAttributeData(Type interfaceType)
    {
        return interfaceType
            .GetCustomAttributes()
            .FirstOrDefault(attr => attr.GetType().IsGenericImplementationOf(typeof(OpcUaEntityMethodsAttribute<>)));
    }
}