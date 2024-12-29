using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Variables;

public static class SupportedVariableTypeCallbackHandler
{
    /// <summary>
    /// Calls the effect if <paramref name="type"/> is supported.
    /// </summary>
    /// <param name="effect">The effect that is invoked if <paramref name="type"/> is supported</param>
    /// <param name="type">A built-in OpcUa type</param>
    /// <typeparam name="T">The result of <paramref name="effect"/>.</typeparam>
    /// <returns>The result produced by invoking <paramref name="effect"/></returns>
    /// <exception cref="NotSupportedException">If <paramref name="type"/> is not supported yet.</exception>
    public static T HandleType<T>(BuiltInType type, Func<T> effect) => HandleType((int)type, effect);

    /// <summary>
    /// Calls the effect if <paramref name="type"/> is supported.
    /// </summary>
    /// <param name="effect">The effect that is invoked if <paramref name="type"/> is supported</param>
    /// <param name="type">A built-in OpcUa type</param>
    /// <typeparam name="T">The result of <paramref name="effect"/>.</typeparam>
    /// <returns>The result produced by invoking <paramref name="effect"/></returns>
    /// <exception cref="NotSupportedException">If <paramref name="type"/> is not supported yet.</exception>
    public static T HandleType<T>(int type, Func<T> effect)
    {
        if (type == DataTypes.Boolean) return effect();
        if (type == DataTypes.Byte) return effect();
        if (type == DataTypes.Int16) return effect();
        if (type == DataTypes.UInt16) return effect();
        if (type == DataTypes.Int32 || type == DataTypes.Integer) return effect();
        if (type == DataTypes.UInt32 || type == DataTypes.UInteger) return effect();
        if (type == DataTypes.Int64) return effect();
        if (type == DataTypes.UInt64) return effect();
        if (type == DataTypes.Float) return effect();
        if (type == DataTypes.Double) return effect();
        if (type == DataTypes.String) return effect();
        if (type == DataTypes.DateTime) return effect();
        if (type == DataTypes.Guid) return effect();
        throw new NotSupportedException(
            $"{Enum.GetName(type.GetType(), type)} is currently not supported type of datavalue.");
    }
    
    
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

    
}