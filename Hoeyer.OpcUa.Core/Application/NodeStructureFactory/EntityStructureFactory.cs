using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
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
        VerifyNoDuplicateMethodNames(methods);
        AssignProperties(properties, entity);
        AssignMethods(methods, entity);

        return new EntityNode(entity,
            new HashSet<PropertyState>(properties.Select(e => e.OpcProperty)),
            new HashSet<MethodState>(methods.Select(e => e.Method)));
    }

    private static void VerifyNoDuplicateMethodNames(IEnumerable<OpcMethodTypeInfo> methods)
    {
        IEnumerable<string> methodNames = methods.Select(e => e.Method.BrowseName.Name);
        List<IGrouping<string, string>> duplicateNames = methodNames.GroupBy(x => x).Where(g => g.Count() > 1).ToList();

        if (duplicateNames.Any())
        {
            Type type = typeof(T);
            throw new InvalidEntityConfigurationException(type.FullName!,
                $"{type.FullName} has the following methods duplicated: {string.Join(", ", duplicateNames)}");
        }
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

    private static IEnumerable<OpcMethodTypeInfo> CreateMethods(Type entityType, BaseObjectState entity)
    {
        return OpcUaEntityTypes
            .EntityBehaviours
            .Where(behaviourService => behaviourService.entity == entityType)
            .SelectMany(behaviourService => behaviourService.service
                .GetMembers()
                .OfType<MethodInfo>())
            .Select(method => new OpcMethodTypeInfo(
                methodName: method.Name,
                parent: entity,
                returnType: method.ReturnType,
                arguments: method.GetParameters()
            ));
    }

    private static IEnumerable<OpcPropertyTypeInfo> CreateProperties(Type entityType, BaseObjectState entity)
    {
        return entityType.GetProperties().Select(e => new OpcPropertyTypeInfo(e, entity));
    }
}