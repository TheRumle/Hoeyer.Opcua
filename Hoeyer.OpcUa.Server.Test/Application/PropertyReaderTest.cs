using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Application;
using Hoeyer.OpcUa.Server.Test.Application.Fixtures;
using Hoeyer.OpcUa.Server.Test.Application.Generators;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application;

[Category(nameof(PropertyReader))]
[EntityFixtureGenerator]
public class PropertyReaderTest(EntityReaderFixture propertyReaderFixture)
{
    private readonly IPropertyReader _propertyReader = new PropertyReader((PermissionType.Browse | PermissionType.Read | PermissionType.Write | PermissionType.ReadRolePermissions | PermissionType.Call | PermissionType.ReceiveEvents));

    public override string ToString()
    {
        return propertyReaderFixture.ToString();
    }

    public IEnumerable<PropertyState> PropertyStates()
    {
        return propertyReaderFixture.Properties;
    }

    public static IEnumerable<TestInput> ObligatoryAttributes()
    {
        return new[]
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
        }.Select(e => new TestInput(e));
    }

    [Test]
    [MatrixDataSource]
    public async Task CanReadProperty(
        [MatrixInstanceMethod<PropertyReaderTest>(nameof(PropertyStates))]
        PropertyState propertyState,
        [MatrixInstanceMethod<PropertyReaderTest>(nameof(ObligatoryAttributes))]
        TestInput attribute)
    {
        var request = new ReadValueId
        {
            AttributeId = attribute
        };

        var result = _propertyReader.ReadProperty(request, propertyState);
        await Assert.That(StatusCode.IsGood(result.ResponseCode)).IsTrue();
    }

    public record TestInput(uint Attribute)
    {
        public static implicit operator uint(TestInput value)
        {
            return value.Attribute;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Attributes.GetBrowseName(Attribute);
        }
    }
}