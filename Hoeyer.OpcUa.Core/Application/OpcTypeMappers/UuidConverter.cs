using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public readonly struct UuidConverter() : IVariantBuilder<Guid>, IVariantBuilder<Uuid>
{
    private readonly IVariantBuilder<Uuid> _inner = new VariantBuilder();

    public Guid GetValue(Variant value)
    {
        var guidVariant = value.ConvertToGuid();
        return Guid.Parse(guidVariant.ToString());
    }

    public Variant WithValue(Uuid value) => _inner.WithValue(value);


    public Variant WithValue(Guid value) => Variant.From(Uuid.Parse(value.ToString()));
    Uuid IVariantBuilder<Uuid>.GetValue(Variant value) => _inner.GetValue(value);
}