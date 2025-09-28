using System.Reflection;
using Hoeyer.OpcUa.Core.Extensions.Reflection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructureFactory;

internal sealed record OpcPropertyTypeInfo : IOpcTypeInfo
{
    public readonly PropertyState OpcProperty;
    public readonly PropertyInfo PropertyInfo;
    public readonly NodeId? TypeId;
    public readonly int ValueRank;

    public OpcPropertyTypeInfo(string browseName, PropertyInfo PropertyInfo, BaseInstanceState parent)
    {
        this.PropertyInfo = PropertyInfo;
        (TypeId, ValueRank) = PropertyInfo.PropertyType.GetOpcTypeInfo();
        OpcProperty = new PropertyState(parent)
        {
            NodeId = new NodeId(parent.BrowseName.Name + "." + PropertyInfo.Name, parent.NodeId.NamespaceIndex),
            BrowseName = browseName,
            DataType = TypeId,
            ValueRank = ValueRank,
            TypeDefinitionId = VariableTypeIds.PropertyType,
            SymbolicName = browseName,
            AccessLevel = AccessLevels.CurrentReadOrWrite,
            UserAccessLevel = AccessLevels.CurrentReadOrWrite,
            MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate,
            ReferenceTypeId = ReferenceTypes.HasProperty,
            DisplayName = browseName
        };
    }

    public BaseInstanceState InstanceState => OpcProperty;
}