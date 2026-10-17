using System.Reflection;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public sealed class EntityNodePropertyAssigner<T>(IBrowseNameCollection<T> entityModel, IPropertyStateFactory<T> propertyStateFactory)
    : IEntityNodePropertyAssigner<T>
{
    private static readonly Type Type = typeof(T);

    public IEnumerable<PropertyState> AssignProperties(BaseObjectState entity)
    {
        var properties = Type
            .GetProperties()
            .Select(e => CreateTypeInfo(entity, e))
            .ToList();
        
        AssignReferences(properties, entity);
        return properties;
    }

    private PropertyState CreateTypeInfo(BaseObjectState entity, PropertyInfo e)
    {
        var browseName = entityModel.PropertyToBrowseName[e.Name];
        return propertyStateFactory.ConstructPropertyState(browseName, e, entity);
    }

    private static void AssignReferences(IEnumerable<BaseInstanceState> values, BaseObjectState entity)
    {
        foreach (var nodeState in values)
        {
            entity.AddChild(nodeState);
            entity.AddReference(ReferenceTypeIds.HasProperty, false, nodeState.NodeId);
        }
    }
}