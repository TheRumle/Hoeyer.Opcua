using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public static class OpcUaAgentTypes
{
    public readonly static FrozenSet<Type> TypesFromReferencingAssemblies = typeof(OpcUaAgentAttribute)
        .GetTypesFromConsumingAssemblies()
        .ToFrozenSet();

    public static readonly FrozenSet<Type> Entities = TypesFromReferencingAssemblies
        .Where(type => type.IsAnnotatedWith<OpcUaAgentAttribute>())
        .ToFrozenSet();


    internal static readonly FrozenSet<(Type service, Type agent)> AgentBehaviours = TypesFromReferencingAssemblies
        .Select(service => (methodService: service,
            attributeInstance: service.GetAnnotationInstance(typeof(OpcUaAgentMethodsAttribute<>))))
        .Where(annotationInstance => annotationInstance.attributeInstance is not null)
        .Select(e => (e.methodService, e.attributeInstance!.GenericTypeArguments[0]))
        .ToFrozenSet();

    internal static readonly FrozenSet<AgentServiceInfo> GenericServices = ServiceTypes
        .Where(type => type.IsGenericType)
        .SelectMany(implementationType =>
        {
            //Find all generic services and create types instantiated version of them
            IEnumerable<OpcUaAgentServiceAttribute> attrs =
                implementationType.GetCustomAttributes<OpcUaAgentServiceAttribute>();
            return attrs.SelectMany(attr =>
            {
                if (implementationType.IsConstructedGenericType)
                {
                    Type? agent = implementationType.GenericTypeArguments[0];
                    return [(implementationType, attr, agent)];
                }

                if (implementationType.IsGenericTypeDefinition)
                {
                    return Entities.Select(agent =>
                    {
                        Type value = implementationType.MakeGenericType(agent);
                        return (implementationType: value, attr, agent);
                    });
                }

                return [];
            });
        })
        .Select(tuple =>
        {
            //For each instantiated service, also create generic versions of the interface
            (Type? implemetationType, OpcUaAgentServiceAttribute? attr, Type? agent) = tuple;
            if (attr.ServiceType.IsConstructedGenericType)
                return new AgentServiceInfo(attr.ServiceType, implemetationType, agent, attr.Lifetime);

            if (attr.ServiceType.IsGenericTypeDefinition)
            {
                Type service = attr.ServiceType.MakeGenericType(agent);
                return new AgentServiceInfo(service, implemetationType, agent, attr.Lifetime);
            }

            return new AgentServiceInfo(attr.ServiceType, implemetationType, agent, attr.Lifetime);
        })
        .ToFrozenSet();

    internal static FrozenSet<Type> ServiceTypes => TypesFromReferencingAssemblies
        .Where(type => type.IsAnnotatedWith<OpcUaAgentServiceAttribute>())
        .ToFrozenSet();

    internal static FrozenSet<AgentServiceInfo> NonGenericServices => ServiceTypes
        .Where(type => !type.IsGenericType)
        .SelectMany(implementationType =>
        {
            //Find all generic services and create types instantiated version of them
            IEnumerable<OpcUaAgentServiceAttribute> attrs =
                implementationType.GetCustomAttributes<OpcUaAgentServiceAttribute>();
            return attrs.SelectMany(attr =>
            {
                if (attr.ServiceType.IsConstructedGenericType)
                {
                    Type? agent = attr.ServiceType.GenericTypeArguments[0];
                    return [new AgentServiceInfo(attr.ServiceType, implementationType, agent, attr.Lifetime)];
                }

                if (attr.ServiceType.IsGenericTypeDefinition)
                {
                    Type implemetation = implementationType
                        .GetInterfaces()
                        .First(e => e.IsGenericType && e.GetGenericTypeDefinition() == attr.ServiceType);

                    Type? agent = implemetation.GenericTypeArguments[0];

                    return [new AgentServiceInfo(implemetation, implementationType, agent, attr.Lifetime)];
                }


                return Enumerable.Empty<AgentServiceInfo>();
            });
        }).Where(e => e is not null)
        .ToFrozenSet();


    internal static FrozenSet<AgentServiceInfo> BehaviourImplementations =>
        ServiceTypes
            .SelectMany(type => AgentBehaviours
                .Where(tuple => type.GetInterfaces().Contains(tuple.service))
                .Select(tuple => new AgentServiceInfo(tuple.service, type, tuple.agent, ServiceLifetime.Singleton)))
            .ToFrozenSet();
};