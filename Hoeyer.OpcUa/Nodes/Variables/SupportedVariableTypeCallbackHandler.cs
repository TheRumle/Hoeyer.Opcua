using System;
using System.Reflection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Nodes.Variables;

public static class SupportedVariableTypeCallbackHandler
{
    /// <summary>
    /// Given a type T, returns the equivalent DataTypes integer variable.
    /// </summary>
    /// See <see cref="DataTypes"/> for a complete list of data types. NOTE: Not all data types in the list are supported.
    /// <exception cref="NotSupportedException">If <paramref name="type"/> is not a supported OpcUa DataType.</exception>
    public static uint ToOpcDataType(this Type type)
    {
        if (type == typeof(bool)) return DataTypes.Boolean;
        if (type == typeof(byte)) return DataTypes.Byte;
        if (type == typeof(short)) return DataTypes.Int16;
        if (type == typeof(ushort)) return DataTypes.UInt16;
        if (type == typeof(int)) return DataTypes.Int32;
        if (type == typeof(uint)) return DataTypes.UInt32;
        if (type == typeof(long)) return DataTypes.Int64;
        if (type == typeof(ulong)) return DataTypes.UInt64;
        if (type == typeof(float)) return DataTypes.Float;
        if (type == typeof(double)) return DataTypes.Double;
        if (type == typeof(string)) return DataTypes.String;
        if (type == typeof(DateTime)) return DataTypes.DateTime;
        if (type == typeof(Uuid)) return DataTypes.Guid;
        throw new NotSupportedException(
            $"The type {type.Name} is  not a supported DataType in OpcUa.");
    }
    
    /// <summary>
    /// Given a type T, returns the equivalent DataTypes integer variable.
    /// </summary>
    /// See <see cref="DataTypes"/> for a complete list of data types. NOTE: Not all data types in the list are supported.
    /// <exception cref="NotSupportedException">If <paramref name="type"/> is not a supported OpcUa DataType.</exception>
    public static uint ToOpcDataType<T>() => ToOpcDataType(typeof(T));

    public static (NodeId nodeId, int ValueRank) ToDataTypeId(PropertyInfo property) => (DataTypeNodeId(property), ValueRanks.Scalar);

    private static NodeId DataTypeNodeId(PropertyInfo property) => new(ToOpcDataType(property.PropertyType));
}