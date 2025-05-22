using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Reflection;
using Microsoft.Extensions.DependencyInjection;

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

    internal static FrozenSet<Type> ServiceTypes => TypesFromReferencingAssemblies
        .Where(type => type.IsAnnotatedWith<OpcUaEntityServiceAttribute>())
        .ToFrozenSet();

    internal static FrozenSet<EntityServiceInfo> GenericServices => ServiceTypes
        .Where(type => type.IsGenericType)
        .SelectMany(implementationType =>
        {
            //Find all generic services and create types instantiated version of them
            IEnumerable<OpcUaEntityServiceAttribute> attrs =
                implementationType.GetCustomAttributes<OpcUaEntityServiceAttribute>();
            return attrs.SelectMany(attr =>
            {
                if (implementationType.IsConstructedGenericType)
                {
                    Type? entity = implementationType.GenericTypeArguments[0];
                    return [(implementationType, attr, entity)];
                }

                if (implementationType.IsGenericTypeDefinition)
                {
                    return Entities.Select(entity =>
                    {
                        Type value = implementationType.MakeGenericType(entity);
                        return (implementationType: value, attr, entity);
                    });
                }

                return [];
            });
        })
        .Select(tuple =>
        {
            //For each instantiated service, also create generic versions of the interface
            (Type? implemetationType, OpcUaEntityServiceAttribute? attr, Type? entity) = tuple;
            if (attr.ServiceType.IsConstructedGenericType)
                return new EntityServiceInfo(attr.ServiceType, implemetationType, entity, attr.Lifetime);

            if (attr.ServiceType.IsGenericTypeDefinition)
            {
                Type service = attr.ServiceType.MakeGenericType(entity);
                return new EntityServiceInfo(service, implemetationType, entity, attr.Lifetime);
            }

            return new EntityServiceInfo(attr.ServiceType, implemetationType, entity, attr.Lifetime);
        })
        .ToFrozenSet();

    internal static FrozenSet<EntityServiceInfo> NonGenericServices => ServiceTypes
        .Where(type => !type.IsGenericType)
        .SelectMany(implementationType =>
        {
            //Find all generic services and create types instantiated version of them
            IEnumerable<OpcUaEntityServiceAttribute> attrs =
                implementationType.GetCustomAttributes<OpcUaEntityServiceAttribute>();
            return attrs.SelectMany(attr =>
            {
                if (attr.ServiceType.IsConstructedGenericType)
                {
                    Type? entity = attr.ServiceType.GenericTypeArguments[0];
                    return [new EntityServiceInfo(attr.ServiceType, implementationType, entity, attr.Lifetime)];
                }

                if (attr.ServiceType.IsGenericTypeDefinition)
                {
                    Type implemetation = implementationType
                        .GetInterfaces()
                        .First(e => e.IsGenericType && e.GetGenericTypeDefinition() == attr.ServiceType);

                    Type? entity = implemetation.GenericTypeArguments[0];

                    return [new EntityServiceInfo(implemetation, implementationType, entity, attr.Lifetime)];
                }


                return Enumerable.Empty<EntityServiceInfo>();
            });
        }).Where(e => e is not null)
        .ToFrozenSet();


    internal static FrozenSet<EntityServiceInfo> BehaviourImplementations =>
        ServiceTypes
            .SelectMany(type => EntityBehaviours
                .Where(tuple => type.GetInterfaces().Contains(tuple.service))
                .Select(tuple => new EntityServiceInfo(tuple.service, type, tuple.entity, ServiceLifetime.Singleton)))
            .ToFrozenSet();
};