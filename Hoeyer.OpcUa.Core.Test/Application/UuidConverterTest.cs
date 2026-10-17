using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Test.Application;

[TestSubject(typeof(UuidConverter))]
public sealed class UuidConverterTest
{
    [Test]
    public async Task CanParseGuidToGuid()
    {
        var value = new Uuid(Guid.NewGuid());
        var guid = new UuidConverter().GetValue(Variant.From(value));
        await Assert.That(guid).IsNotEqualTo(Guid.Empty);
        await Assert.That(guid.ToString()).IsEqualTo(value.ToString());
    }
}