using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Test.Fixtures.Application.NodeServices;
using Hoeyer.OpcUa.Server.Test.Generators;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application;

[EntityPropertyFixtureGenerator]
public class PropertyReaderTest(EntityPropertyFixture propertyFixture)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return propertyFixture.ToString();
    }

    private readonly IPropertyReader _propertyReader = new PropertyReader();
    public static IEnumerable<(uint, string)> ObligatoryAttributes() => new[]
    {
        Attributes.BrowseName,
        Attributes.NodeId,
        Attributes.NodeClass,
        Attributes.DisplayName,
        Attributes.Description,
        Attributes.Value,
        Attributes.ValueRank,
        Attributes.DataType,
        Attributes.MinimumSamplingInterval
    }.Select(e=> (e, Attributes.GetBrowseName(e)));



    [Test]
    [MethodDataSource(nameof(ObligatoryAttributes))]
    public async Task CanReadProperty(uint attribute, string _)
    {
        var request = new ReadValueId
        {
            AttributeId = attribute
        };

        var result = _propertyReader.ReadProperty(request, propertyFixture.PropertyState);
        await Assert.That(StatusCode.IsGood(result.ResponseCode)).IsTrue();
    }
}