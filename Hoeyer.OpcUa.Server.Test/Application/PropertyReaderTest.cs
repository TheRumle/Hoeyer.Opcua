using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Test.Fixtures.Application;
using Hoeyer.OpcUa.Server.Test.Fixtures.Generators;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application;

[Category(nameof(PropertyReader))]
[PropertyFixtureGenerator]
public class PropertyReaderTest(PropertyReaderFixture propertyReaderFixture)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return propertyReaderFixture.ToString();
    }

    private readonly IPropertyReader _propertyReader = new PropertyReader();
    public static IEnumerable<TestInput> ObligatoryAttributes() => new[]
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
    }.Select(e=> new TestInput(e));

    public readonly record struct TestInput(uint Attribute)
    {
        public static implicit operator uint(TestInput value) => value.Attribute;

        /// <inheritdoc />
        public override string ToString()
        {
            return Attributes.GetBrowseName(Attribute);
        }
    } 

    [Test]
    [MethodDataSource(nameof(ObligatoryAttributes))]
    [DisplayName("$attribute")]
    public async Task CanReadProperty(TestInput attribute)
    {
        var request = new ReadValueId
        {
            AttributeId = attribute
        };

        var result = _propertyReader.ReadProperty(request, propertyReaderFixture.PropertyState);
        await Assert.That(StatusCode.IsGood(result.ResponseCode)).IsTrue();
    }
}