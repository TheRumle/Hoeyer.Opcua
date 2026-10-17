using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Test.Application;

[TestSubject(typeof(EnumVariantConverter<TestEnum>))]
public sealed class EnumVariantConverterTest
{
    private enum TestEnum
    {
        First = 1,
        Second = 10
    }

    [Test]
    public async Task CanParseEnumToEnum()
    {
        var value = TestEnum.Second;
        var variant = Variant.From(value);

        var result = new EnumVariantConverter<TestEnum>()
            .GetValue(variant);

        await Assert.That(result).IsEqualTo(value);
    }

    [Test]
    public async Task CanConvertEnumToVariant()
    {
        var value = TestEnum.Second;

        var variant = new EnumVariantConverter<TestEnum>()
            .WithValue(value);

        var result = new EnumVariantConverter<TestEnum>()
            .GetValue(variant);

        await Assert.That(result).IsEqualTo(value);
    }

    [Test]
    public async Task ThrowsWhenVariantIsNotAnEnum()
    {
        var variant = Variant.From("Not an enum");

        await Assert.That(() =>
                new EnumVariantConverter<TestEnum>().GetValue(variant))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ThrowsWhenEnumValueIsUndefined()
    {
        var variant = Variant.From(999);
        variant.ConvertTo(BuiltInType.Enumeration);

        await Assert.That(() =>
                new EnumVariantConverter<TestEnum>().GetValue(variant))
            .Throws<InvalidOperationException>();
    }
}