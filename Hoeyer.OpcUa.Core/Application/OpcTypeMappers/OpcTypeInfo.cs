using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public record struct OpcTypeInfo(BuiltInType BuiltInType, int Rank, NodeId TypeId)
{
    public static OpcTypeInfo FromType(Type type)
    {
        var rank = TypeInfo.GetValueRank(type);
        var typeId = TypeInfo.GetDataTypeId(type);
        var builtInType = TypeInfo.GetBuiltInType(typeId);
        return new(builtInType, rank, typeId);
    }
}