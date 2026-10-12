using Opc.Ua;
using TypeInfo = Opc.Ua.TypeInfo;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public static class OpcTypeInfoUtils
{
    public static OpcTypeInfo ConstructOpcTypeInfo(Type type)
    {
        var rank = TypeInfo.GetValueRank(type);
        var typeId = TypeInfo.GetDataTypeId(type);
        var builtInType = TypeInfo.GetBuiltInType(typeId);
        return new(builtInType, rank, typeId);
    }

    public static (NodeId TypeId, int Rank) ToOpcTypeTuple(this Type type)
    {
        var data = ConstructOpcTypeInfo(type);
        return (data.TypeId, data.Rank);
    }
}