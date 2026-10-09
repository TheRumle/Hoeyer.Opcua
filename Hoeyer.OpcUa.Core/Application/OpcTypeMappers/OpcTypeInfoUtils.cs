using System.Reflection;
using Opc.Ua;
using TypeInfo = Opc.Ua.TypeInfo;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public static class OpcTypeInfoUtils
{
    public static PropertyState CreateGenericProperty<T>(
        string browseName,
        NodeId nodeId,
        PropertyInfo propertyInfo,
        BaseInstanceState parent
    )
    {
        var typeInfo = ConstructOpcTypeInfo(propertyInfo.PropertyType);
        return new PropertyState<T>(parent)
        {
            NodeId = nodeId,
            BrowseName = browseName,
            DataType = typeInfo.TypeId,
            ValueRank = typeInfo.Rank,
            TypeDefinitionId = VariableTypeIds.PropertyType,
            SymbolicName = browseName,
            AccessLevel = AccessLevels.CurrentReadOrWrite,
            UserAccessLevel = AccessLevels.CurrentReadOrWrite,
            MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate,
            ReferenceTypeId = ReferenceTypes.HasProperty,
            DisplayName = browseName
        };
    }
    
    public static OpcTypeInfo ConstructOpcTypeInfo(Type type)
    {
        var rank = TypeInfo.GetValueRank(type);
        var typeId = TypeInfo.GetDataTypeId(type);
        var builtInType = TypeInfo.GetBuiltInType(typeId);
        return new (builtInType, rank, typeId);
    }
    
    public static (NodeId TypeId, int Rank) ToOpcTypeTuple(this Type type)
    {
        var data = OpcTypeInfoUtils.ConstructOpcTypeInfo(type);
        return (data.TypeId, data.Rank);
    }
}