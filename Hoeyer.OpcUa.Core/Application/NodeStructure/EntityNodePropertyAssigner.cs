using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public sealed class EntityNodePropertyAssigner<T>(IBrowseNameCollection<T> entityModel)
    : IEntityNodePropertyAssigner<T>
{
    private static readonly Type Type = typeof(T);

    public IEnumerable<PropertyState> AssignProperties(BaseObjectState entity)
    {
        var properties = Type
            .GetProperties()
            .Select(e => CreateTypeInfo(entity, e))
            .ToList();
        var errors = VerifyProperties(properties);
        if (errors.Length != 0)
        {
            throw new AggregateException(errors);
        }

        AssignReferences(properties, entity);
        return properties.Select(e => e.OpcProperty);
    }

    private OpcPropertyTypeInfo CreateTypeInfo(BaseObjectState entity, PropertyInfo e) =>
        new(entityModel.PropertyNames[e.Name], e, entity);

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

    private static Exception[] VerifyProperties(IList<OpcPropertyTypeInfo> properties)
    {
        return properties
            .Where(e => e.TypeId is null)
            .Select(Exception (e) =>
                new InvalidEntityConfigurationException(
                    Type.FullName!,
                    $"The property {e.PropertyInfo.Name} is of type {e.PropertyInfo.PropertyType.FullName} and could not be translated to NodeId representing the type."))
            .ToArray();
    }
}