using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public sealed class AggregateVariantConverter(VariantConverterCollection converterCollection)
{
    public Variant Convert<T>(T data) => converterCollection.Get<T>().WithValue(data);
    public Variant Convert<T, TCollection>(TCollection data) where TCollection : IEnumerable<T> => ConvertCollection(data);

    public Variant ConvertCollection<T>(IEnumerable<T> data)
    {
        var converter = converterCollection.Get<T>();
        return Variant.From(data.Select(converter.WithValue).ToArrayOf());
    }
}