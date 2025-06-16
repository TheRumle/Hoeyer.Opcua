using System;
using System.Globalization;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.Translator;

public static class OpcToCSharpValueParser
{
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
            BuiltInType.SByte => value.Value,
            BuiltInType.Byte => value.Value,
            BuiltInType.Int16 => value.Value,
            BuiltInType.UInt16 => value.Value,
            BuiltInType.Int32 => value.Value,
            BuiltInType.UInt32 => value.Value,
            BuiltInType.Int64 => value.Value,
            BuiltInType.UInt64 => value.Value,
            BuiltInType.Float => value.Value,
            BuiltInType.Double => value.Value,
            BuiltInType.String => value.Value,
            BuiltInType.ByteString => value.Value,
            BuiltInType.XmlElement => value.Value,
            BuiltInType.NodeId => value.Value,
            BuiltInType.ExpandedNodeId => value.Value,
            BuiltInType.StatusCode => value.Value,
            BuiltInType.QualifiedName => value.Value,
            BuiltInType.LocalizedText => value.Value,
            BuiltInType.ExtensionObject => value.Value,
            BuiltInType.DataValue => value.Value,
            BuiltInType.Variant => value.Value,
            BuiltInType.DiagnosticInfo => value.Value,
            BuiltInType.Number => value.Value,
            BuiltInType.Integer => value.Value,
            BuiltInType.UInteger => value.Value,
            BuiltInType.Enumeration => value.Value,
            BuiltInType.Null => null,
            BuiltInType.DateTime => DateTime.Parse((string)value.Value, DateTimeFormatInfo.InvariantInfo),
            BuiltInType.Guid => value.Value is Guid guid ? guid : ParseUuid((Uuid)value.Value),
            var _ => throw new ArgumentOutOfRangeException(value.ToString() +
                                                           " could not be parsed to a meaningful value")
        };
    }

    public static Guid ParseUuid(Uuid uuid) => Guid.Parse(uuid.GuidString);
}