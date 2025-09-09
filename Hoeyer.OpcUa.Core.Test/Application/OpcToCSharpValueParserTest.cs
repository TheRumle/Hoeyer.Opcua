using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Test.Application;

[TestSubject(typeof(OpcToCSharpValueParser))]
public sealed class OpcToCSharpValueParserTest
{
    [Test]
    public async Task CanParseGuidToGuid()
    {
        var value = new Uuid();
        var guid = OpcToCSharpValueParser.ParseTo<Guid>(value);
        await Assert.That(guid).IsNotEqualTo(Guid.Empty);
    }
}