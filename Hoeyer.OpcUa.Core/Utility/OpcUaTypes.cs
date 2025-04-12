using System;
using System.Xml;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Utility;

public static class OpcUaTypes
{
    public static Type ToType(BuiltInType dataValue)
    {
        return dataValue switch
        {
            BuiltInType.Integer => typeof(int),
            BuiltInType.UInteger => typeof(uint),
            BuiltInType.Boolean => typeof(bool),
            BuiltInType.SByte => typeof(sbyte),
            BuiltInType.Byte => typeof(byte),
            BuiltInType.Int16 => typeof(ushort),
            BuiltInType.UInt16 => typeof(ushort),
            BuiltInType.Int32 => typeof(int),
            BuiltInType.UInt32 => typeof(uint),
            BuiltInType.Int64 => typeof(long),
            BuiltInType.UInt64 => typeof(ulong),
            BuiltInType.Float => typeof(float),
            BuiltInType.Double => typeof(double),
            BuiltInType.String => typeof(string),
            BuiltInType.DateTime => typeof(DateTime),
            BuiltInType.Guid => typeof(DateTime),
            BuiltInType.ByteString => typeof(byte[]),
            BuiltInType.XmlElement => typeof(XmlElement),
            BuiltInType.NodeId => typeof(NodeId),
            BuiltInType.ExpandedNodeId => typeof(ExpandedNodeId),
            BuiltInType.StatusCode => typeof(StatusCode),
            BuiltInType.QualifiedName => typeof(QualifiedName),
            BuiltInType.LocalizedText => typeof(LocalizedText),
            BuiltInType.ExtensionObject => typeof(ExtensionObject),
            BuiltInType.DataValue => typeof(DataValue),
            BuiltInType.Variant => typeof(Variant),
            BuiltInType.DiagnosticInfo => typeof(DiagnosticInfo),
            BuiltInType.Number => typeof(NumericRange),
            BuiltInType.Enumeration => typeof(Enum),
            BuiltInType.Null =>
                throw new ArgumentException($"Type is {nameof(BuiltInType)}.{nameof(BuiltInType.Null)}"),
            _ => throw new ArgumentOutOfRangeException("Type was an unexpected value: " + dataValue)
        };
    }
}