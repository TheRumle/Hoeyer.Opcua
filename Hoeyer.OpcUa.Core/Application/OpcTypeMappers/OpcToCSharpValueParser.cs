using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public static class OpcToCSharpValueParser
{
    public static T ParseTo<T>(object val) => (T)ParseOpcValue(val)!;

    public static T ParseTo<T>(Variant variant) => (T)ParseOpcValue(variant)!;

    public static T[] ParseToArray<T>(Variant variant)
    {
        if (variant.TypeInfo.ValueRank != ValueRanks.OneDimension)
            throw new ArgumentException("Variant must be one dimension.");

        if (variant.Value is T[] asArray)
        {
            return asArray;
        }

        if (variant.Value is IEnumerable<T> asEnumerable)
        {
            return asEnumerable.ToArray();
        }

        var values = variant.Value as IEnumerable<object>;
        if (values is null)
        {
            throw new ArgumentException("Variant value was not IEnumerable or array type.");
        }

        return values.Select(ParseTo<T>).ToArray();
    }

    public static object? ParseOpcValue(object val)
    {
        return val switch
        {
            Guid guid => guid,
            DateTime dt => dt,
            DataValue dv => ParseOpcValue(dv.WrappedValue),
            Variant v => ParseOpcValue(v),
            Uuid uuid => ParseUuid(uuid),
            var _ => val
        };
    }

    public static object? ParseOpcValue(Variant value)
    {
        return value.TypeInfo.BuiltInType switch
        {
            BuiltInType.Boolean => value.Value,
            BuiltInType.Null => null,
            BuiltInType.DateTime => DateTime.Parse((string)value.Value, DateTimeFormatInfo.InvariantInfo),
            BuiltInType.Guid => value.Value is Guid guid ? guid : ParseUuid((Uuid)value.Value),
            var _ => value.Value
        };
    }

    public static Guid ParseUuid(Uuid uuid) => Guid.Parse(uuid.GuidString);
}