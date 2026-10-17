using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public sealed class AggregateVariantConverter(VariantConverterCollection converterCollection)
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
    public Variant ConvertCollectionVariant(IEnumerable<DateTime> data) => Variant.From(data.Select(DateTimeUtc.From).ToArray());
    public Variant ConvertCollectionVariant(IEnumerable<Guid> data) => Variant.From(data.Select(id => Uuid.Parse(id.ToString())).ToArray());
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
}