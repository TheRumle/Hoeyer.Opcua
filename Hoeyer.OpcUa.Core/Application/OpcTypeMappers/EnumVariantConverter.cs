using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public readonly struct EnumVariantConverter<T> : IVariantBuilder<T> 
    where T : struct, Enum
{
    public T GetValue(Variant value)
    {
        if (value.ValueIsDefaultOrNull ||
            !value.TryGetValue(out EnumValue enumeration))
        {
            throw new InvalidOperationException("The variant was not an enumeration");
        }

        return Enum.IsDefined(typeof(T), enumeration.Value)
            ? (T)Enum.ToObject(typeof(T), enumeration.Value)
            : throw new InvalidOperationException($"Invalid enum value: {enumeration.Value}({enumeration.Symbol ?? "No string representation"})");
    }
    
    public Variant WithValue(T value)
    {
        var v = Variant.From(value);
        v.ConvertTo(BuiltInType.Enumeration);
        return v;
    }
}