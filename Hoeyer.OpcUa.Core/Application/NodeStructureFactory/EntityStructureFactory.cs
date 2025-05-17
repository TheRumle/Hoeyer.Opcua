using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructureFactory;

[OpcUaEntityService(typeof(IEntityNodeStructureFactory<>), ServiceLifetime.Singleton)]
public class EntityStructureFactory<T> : IEntityNodeStructureFactory<T>
{
    /// <inheritdoc />
    public IEntityNode Create(ushort applicationNamespaceIndex)
    {
        var type = typeof(T);
        var entityName = type.Name;
        BaseObjectState entity = new BaseObjectState(null)
        {
            BrowseName = new QualifiedName(entityName, applicationNamespaceIndex),
            NodeId = new NodeId(entityName, applicationNamespaceIndex),
            DisplayName = entityName,
        };
        entity.AccessRestrictions = AccessRestrictionType.None;

        IEnumerable<OpcPropertyTypeInfo> properties = CreateProperties(type, entity).ToList();
        IEnumerable<OpcMethodTypeInfo> methods = CreateMethods(type, entity).ToList();

        AssignProperties(properties, entity);
        AssignMethods(methods, entity);

        return new EntityNode(entity,
            new HashSet<PropertyState>(properties.Select(e => e.OpcProperty)),
            new HashSet<MethodState>(methods.Select(e => e.Method)));
    }

    private static void AssignMethods(IEnumerable<IOpcTypeInfo> methods, BaseObjectState entity)
    {
        foreach (var pr in methods.Select(type => type.InstanceState))
        {
            entity.AddReference(ReferenceTypeIds.HasComponent, false, pr.NodeId);
            entity.AddChild(pr);
        }
    }

    private static void AssignProperties(IEnumerable<IOpcTypeInfo> values, BaseObjectState entity)
    {
        foreach (var pr in values)
        {
            var referenceTypeid = pr switch
            {
                OpcMethodTypeInfo => ReferenceTypeIds.HasComponent,
                OpcPropertyTypeInfo => ReferenceTypeIds.HasProperty,
                _ => throw new ArgumentOutOfRangeException(pr.GetType().Name + " is not supported!")
            };
            entity.AddChild(pr.InstanceState);
            entity.AddReference(referenceTypeid, false, pr.InstanceState.NodeId);
        }
    }

    private static IEnumerable<OpcMethodTypeInfo> CreateMethods(Type type, BaseObjectState entity)
    {
        var delegateMethods = type.GetNestedTypes(BindingFlags.Public)
            .Where(definedType => typeof(Delegate).IsAssignableFrom(definedType))
            .Select(t => (name: t.Name, delegateType: t, invokeMethod: t.GetMethod("Invoke")));

        var events = type
            .GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(e => (name: e.Name, delegateType: e.EventHandlerType,
                invokeMethod: e.EventHandlerType?.GetMethod("Invoke")));

        var methodCandidates = delegateMethods
            .Concat(events)
            .Where(nameAndHandler => nameAndHandler.delegateType != null
                                     && typeof(Delegate).IsAssignableFrom(nameAndHandler.delegateType)
                                     && nameAndHandler.invokeMethod != null)
            .Select(e => (e.name, e.invokeMethod));

        return methodCandidates.Select(delegateTypeInfo =>
        {
            var (name, invokeMethod) = delegateTypeInfo;
            (Type ParameterType, string)[] parameters = invokeMethod!.GetParameters()
                .Select(p => (p.ParameterType, p.Name ?? "arg" + p.Position))
                .ToArray();
            var returnType = invokeMethod.ReturnType == typeof(void) ? null : invokeMethod.ReturnType;
            return new OpcMethodTypeInfo(name, parameters, returnType, entity);
        });
    }

    private static IEnumerable<OpcPropertyTypeInfo> CreateProperties(Type type, BaseObjectState entity)
    {
        return type.GetProperties().Select(e => new OpcPropertyTypeInfo(e, entity));
    }
}