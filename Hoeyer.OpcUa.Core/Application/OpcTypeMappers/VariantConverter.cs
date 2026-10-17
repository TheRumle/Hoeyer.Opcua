using System.Diagnostics.CodeAnalysis;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

[SuppressMessage("Maintainability", checkId: "S2325",
    Justification = "The functional surface is used in generated code and must be injectable")]
public sealed class VariantConverter(VariantConverterCollection converterCollection)
{
    public Variant ConvertCollectionVariant(IEnumerable<bool> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<sbyte> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<byte> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<short> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<ushort> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<int> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<uint> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<long> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<ulong> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<float> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<double> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<string> data) => Variant.From(data.ToArray());

    public Variant ConvertCollectionVariant(IEnumerable<DateTime> data) =>
        Variant.From(data.Select(DateTimeUtc.From).ToArray());

    public Variant ConvertCollectionVariant(IEnumerable<Guid> data) =>
        Variant.From(data.Select(id => Uuid.Parse(id.ToString())).ToArray());

    public Variant ConvertCollectionVariant(IEnumerable<XmlElement> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<NodeId> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<ExpandedNodeId> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<StatusCode> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<QualifiedName> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<LocalizedText> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<ExtensionObject> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<DataValue> data) => Variant.From(data.ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<Variant> data) => Variant.From(data.ToArray());
    public Variant ConvertToVariant<T>(T data) => converterCollection.Get<T>().WithValue(data);
    public T ConvertToType<T>(Variant v) => converterCollection.Get<T>().GetValue(v);

    public TCollection ConvertToCollectionType<T, TCollection>(Variant v) where TCollection : ICollection<T>, new()
    {
        var casted = v.CastTo<T[]>(throwOnError: true);
        var collection = new TCollection();
        foreach (var item in casted)
        {
            collection.Add(item);
        }

        return collection;
    }

    public object ConvertToObject(Variant variant)
    {
        var builtInType = variant.TypeInfo.BuiltInType;
        if (variant.TypeInfo.ValueRank == ValueRanks.Scalar)
        {
            return ConvertToObject(variant, builtInType);
        }

        if (variant.TypeInfo.ValueRank == ValueRanks.OneDimension)
        {
            return ConvertToList(variant, builtInType);
        }

        throw new ArgumentOutOfRangeException(nameof(variant),
            $"The {nameof(Variant)}s {nameof(variant.TypeInfo.ValueRank)} ({variant.TypeInfo.ValueRank}) is not supported. " +
            $"Only {nameof(ValueRanks.Scalar)} ({ValueRanks.Scalar}) and {nameof(ValueRanks.OneDimension)} ({ValueRanks.OneDimension}) is supported");
    }

    private List<object> ConvertToList(Variant variant, BuiltInType builtInType)
    {
        return builtInType switch
        {
            BuiltInType.Null => throw new InvalidOperationException("Cannot convert null variant to object"),
            BuiltInType.Boolean => ConvertToObjectList<bool>(variant),
            BuiltInType.SByte => ConvertToObjectList<sbyte>(variant),
            BuiltInType.Byte => ConvertToObjectList<byte>(variant),
            BuiltInType.Int16 => ConvertToObjectList<short>(variant),
            BuiltInType.UInt16 => ConvertToObjectList<ushort>(variant),
            BuiltInType.Int32 => ConvertToObjectList<int>(variant),
            BuiltInType.UInt32 => ConvertToObjectList<uint>(variant),
            BuiltInType.Int64 => ConvertToObjectList<ulong>(variant),
            BuiltInType.UInt64 => ConvertToObjectList<ulong>(variant),
            BuiltInType.Float => ConvertToObjectList<float>(variant),
            BuiltInType.Double => ConvertToObjectList<double>(variant),
            BuiltInType.String => ConvertToObjectList<string>(variant),
            BuiltInType.DateTime => ConvertToObjectList<DateTime>(variant),
            BuiltInType.Guid => ConvertToObjectList<Uuid>(variant),
            BuiltInType.NodeId => ConvertToObjectList<NodeId>(variant),
            BuiltInType.StatusCode => ConvertToObjectList<StatusCode>(variant),
            BuiltInType.Number => ConvertToObjectList<double>(variant),
            BuiltInType.Integer => ConvertToObjectList<int>(variant),
            BuiltInType.UInteger => ConvertToObjectList<uint>(variant),
            BuiltInType.Enumeration => ConvertToObjectList<uint>(variant),
            _ => throw new ArgumentOutOfRangeException(nameof(builtInType),
                $"The BuiltInType {builtInType.ToString()} is currently not supported")
        };
    }

    private List<object> ConvertToObjectList<T>(Variant variant) => ConvertToCollectionType<T, List<T>>(variant).Select(e => (object)e!).ToList();

    private object ConvertToObject(Variant variant, BuiltInType builtInType)
    {
        return builtInType switch
        {
            BuiltInType.Null => throw new InvalidOperationException("Cannot convert null variant to object"),
            BuiltInType.Boolean => ConvertToType<bool>(variant),
            BuiltInType.SByte => ConvertToType<sbyte>(variant),
            BuiltInType.Byte => ConvertToType<byte>(variant),
            BuiltInType.Int16 => ConvertToType<short>(variant),
            BuiltInType.UInt16 => ConvertToType<ushort>(variant),
            BuiltInType.Int32 => ConvertToType<int>(variant),
            BuiltInType.UInt32 => ConvertToType<uint>(variant),
            BuiltInType.Int64 => ConvertToType<ulong>(variant),
            BuiltInType.UInt64 => ConvertToType<ulong>(variant),
            BuiltInType.Float => ConvertToType<float>(variant),
            BuiltInType.Double => ConvertToType<double>(variant),
            BuiltInType.String => ConvertToType<string>(variant),
            BuiltInType.DateTime => ConvertToType<DateTime>(variant),
            BuiltInType.Guid => ConvertToType<Uuid>(variant),
            BuiltInType.NodeId => ConvertToType<NodeId>(variant),
            BuiltInType.StatusCode => ConvertToType<StatusCode>(variant),
            BuiltInType.Number => ConvertToType<double>(variant),
            BuiltInType.Integer => ConvertToType<int>(variant),
            BuiltInType.UInteger => ConvertToType<uint>(variant),
            BuiltInType.Enumeration => ConvertToType<uint>(variant),
            _ => throw new ArgumentOutOfRangeException(nameof(builtInType),
                $"The BuiltInType {builtInType} is currently not supported")
        };
    }
}