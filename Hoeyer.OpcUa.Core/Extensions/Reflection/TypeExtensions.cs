using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Extensions.Reflection;

public static class TypeExtensions
{
    public static (NodeId typeId,  int rank) GetOpcTypeInfo(this Type t)
    {
        var collectionInterface = t.GetInterfaces().FirstOrDefault(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(ICollection<>));

        var valueRank = TypeInfo.GetValueRank(t);
        if (collectionInterface is null)
        {
            return (TypeInfo.GetDataTypeId(t), valueRank);
        }

        var gottenId = TypeInfo.GetDataTypeId(collectionInterface.GenericTypeArguments[0]);
        return (gottenId, valueRank);

    }
    
}