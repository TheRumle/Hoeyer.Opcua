using System.Reflection;
using Hoeyer.OpcUa.Core.Extensions.Reflection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructureFactory;

internal sealed record OpcPropertyTypeInfo : IOpcTypeInfo
{
    public OpcPropertyTypeInfo(PropertyInfo PropertyInfo, BaseInstanceState parent)
    {
        this.PropertyInfo = PropertyInfo;
        (TypeId, ValueRank) = PropertyInfo.PropertyType.GetOpcTypeInfo();
        var propertyName = PropertyInfo.Name;
        OpcProperty = new PropertyState(parent)
        {
            NodeId = new NodeId(PropertyInfo.Name + "." + parent.BrowseName.Name, parent.NodeId.NamespaceIndex),
            BrowseName = propertyName,
            DataType = TypeId,
            ValueRank = ValueRank,
            TypeDefinitionId =  VariableTypeIds.PropertyType,
            SymbolicName = propertyName,
            AccessLevel = AccessLevels.CurrentReadOrWrite,
            UserAccessLevel = AccessLevels.CurrentReadOrWrite,
            MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate,
            ReferenceTypeId = ReferenceTypes.HasProperty,
            DisplayName = propertyName,
        };
    }

    public readonly PropertyState OpcProperty;
    public readonly int ValueRank;
    public readonly NodeId TypeId;
    public readonly PropertyInfo PropertyInfo;
    public BaseInstanceState InstanceState => OpcProperty;
}