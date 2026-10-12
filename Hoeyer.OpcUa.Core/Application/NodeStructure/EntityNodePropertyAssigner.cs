using System.Reflection;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
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
        return properties.Select(e => e.OpcProperty);
    }

    private OpcPropertyTypeInfo CreateTypeInfo(BaseObjectState entity, PropertyInfo e)
    {
        var browseName = entityModel.PropertyToBrowseName[e.Name];
        var state = propertyStateFactory.ConstructPropertyState(browseName, e, entity);
        return new(e,state);
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
}