using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Extensions.Reflection;

public static class TypeExtensions
{
    public static (NodeId? typeId, int rank) GetOpcTypeInfo(this Type t)
    {
        var valueRank = TypeInfo.GetValueRank(t);
        var collectionInterface = t.GetInterfaces().FirstOrDefault(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(ICollection<>));
        if (collectionInterface is not null)
        {
            NodeId? gottenId = TypeInfo.GetDataTypeId(collectionInterface.GenericTypeArguments[0]);
            return (gottenId, valueRank);
        }

        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>))
            return GetOpcTypeInfo(t.GenericTypeArguments[0]);

        if (t == typeof(Task)) return (null!, valueRank);

        return (TypeInfo.GetDataTypeId(t), valueRank);
    }
}