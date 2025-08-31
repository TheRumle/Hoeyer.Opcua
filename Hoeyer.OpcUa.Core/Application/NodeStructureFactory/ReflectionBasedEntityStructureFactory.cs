using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructureFactory;

public class ReflectionBasedEntityStructureFactory<T> : IEntityNodeStructureFactory<T>
{
    private readonly Type _type = typeof(T);

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

        List<OpcPropertyTypeInfo> properties = CreateProperties(type, entity).ToList();
        List<OpcMethodTypeInfo> methods = CreateMethods(type, entity).ToList();
        Exception[] errors = VerifyNoDuplicateMethodNames(methods)
            .Union(VerifyProperties(properties))
            .ToArray();

        if (errors.Length != 0) throw new AggregateException(errors);
        AssignReferences(properties, entity);
        AssignMethods(methods, entity);

        return new EntityNode(entity,
            new HashSet<PropertyState>(properties.Select(e => e.OpcProperty)),
            new HashSet<MethodState>(methods.Select(e => e.Method)));
    }

    private IEnumerable<Exception> VerifyProperties(IList<OpcPropertyTypeInfo> properties)
    {
        return properties
            .Where(e => e.TypeId is null)
            .Select(e =>
                new InvalidEntityConfigurationException(_type.FullName!,
                    $"The property {e.PropertyInfo.Name} is of type {e.PropertyInfo.PropertyType.FullName} and could not be translated to NodeId representing the type."));
    }

    private IEnumerable<Exception> VerifyNoDuplicateMethodNames(IList<OpcMethodTypeInfo> methods)
    {
        IEnumerable<string> methodNames = methods.Select(e => e.Method.BrowseName.Name);
        List<IGrouping<string, string>> duplicateNames = methodNames.GroupBy(x => x).Where(g => g.Count() > 1).ToList();
        return duplicateNames
            .Select(name => new InvalidEntityConfigurationException(
                _type.FullName!,
                $"{_type.FullName} has multiple definitions of the following method: {name}"));
    }

    private static void AssignMethods(IEnumerable<IOpcTypeInfo> methods, BaseObjectState entity)
    {
        foreach (var pr in methods.Select(type => type.InstanceState))
        {
            entity.AddReference(ReferenceTypeIds.HasComponent, false, pr.NodeId);
            entity.AddChild(pr);
        }
    }

    private static void AssignReferences(IEnumerable<IOpcTypeInfo> values, BaseObjectState entity)
    {
        var exceptions = new List<Exception>();
        foreach (var pr in values)
        {
            var referenceTypeid = pr switch
            {
                OpcMethodTypeInfo => ReferenceTypeIds.HasComponent,
                OpcPropertyTypeInfo => ReferenceTypeIds.HasProperty,
                var _ => throw new ArgumentOutOfRangeException(pr.GetType().Name + " is not a handled case")
            };

            entity.AddChild(pr.InstanceState);
            entity.AddReference(referenceTypeid, false, pr.InstanceState.NodeId);
        }

        if (exceptions.Any()) throw new AggregateException(exceptions);
    }

    private static IEnumerable<OpcMethodTypeInfo> CreateMethods(Type entityType, BaseObjectState entity)
    {
        return OpcUaEntityTypes
            .EntityBehaviours
            .Where(behaviourService => behaviourService.Entity == entityType)
            .SelectMany(behaviourService => behaviourService.ServiceInterface
                .GetMembers()
                .OfType<MethodInfo>())
            .Select(method => new OpcMethodTypeInfo(
                methodName: method.Name,
                parent: entity,
                returnType: method.ReturnType == typeof(Task) && !method.ReturnType.IsGenericType
                    ? null
                    : method.ReturnType,
                arguments: method.GetParameters()
            ));
    }

    private static IEnumerable<OpcPropertyTypeInfo> CreateProperties(Type entityType, BaseObjectState entity)
    {
        return entityType.GetProperties().Select(e => new OpcPropertyTypeInfo(e, entity));
    }
}