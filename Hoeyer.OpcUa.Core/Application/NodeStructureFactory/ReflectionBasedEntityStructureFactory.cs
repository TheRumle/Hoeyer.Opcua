using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructureFactory;

public sealed class ReflectionBasedEntityStructureFactory<T>(IBrowseNameCollection<T> browseNameCollection)
    : IEntityNodeStructureFactory<T>
{
    private readonly Type _type = typeof(T);
    private IEntityNode? _node;

    /// <inheritdoc />
    public IEntityNode Create(ushort applicationNamespaceIndex)
    {
        if (_node != null)
        {
            return _node;
        }

        var type = typeof(T);
        var browseName = browseNameCollection.EntityName;

        BaseObjectState entity = new BaseObjectState(null)
        {
            BrowseName = new QualifiedName(browseName, applicationNamespaceIndex),
            NodeId = new NodeId(browseName, applicationNamespaceIndex),
            DisplayName = browseName
        };
        entity.AccessRestrictions = AccessRestrictionType.None;

        var properties = type
            .GetProperties()
            .Select(e => new OpcPropertyTypeInfo(browseNameCollection.PropertyNames[e.Name], e, entity))
            .ToList();

        List<OpcMethodTypeInfo> methods = CreateMethods(type, entity).ToList();
        Exception[] errors = VerifyNoDuplicateMethodNames(methods)
            .Union(VerifyProperties(properties))
            .ToArray();

        if (errors.Length != 0) throw new AggregateException(errors);
        AssignReferences(properties, entity);
        AssignMethods(methods, entity);

        _node = new EntityNode(entity,
            new HashSet<PropertyState>(properties.Select(e => e.OpcProperty)),
            new HashSet<MethodState>(methods.Select(e => e.Method)));
        return _node;
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
    }

    private IEnumerable<OpcMethodTypeInfo> CreateMethods(Type entityType, BaseObjectState entity)
    {
        return OpcUaEntityTypes.MethodsByEntity[entityType]
            .Select(method =>
            {
                var browseName = browseNameCollection.MethodNames[method.Name];
                return new OpcMethodTypeInfo(
                    methodName: browseName,
                    parent: entity,
                    returnType: method.ReturnType == typeof(Task) && !method.ReturnType.IsGenericType
                        ? null
                        : method.ReturnType,
                    arguments: method.GetParameters()
                );
            });
    }
}