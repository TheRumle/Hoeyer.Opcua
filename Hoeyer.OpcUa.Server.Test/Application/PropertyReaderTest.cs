using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Application.RequestResponse;
using Hoeyer.OpcUa.Server.Test.Fixtures.Application;
using Hoeyer.OpcUa.Server.Test.Fixtures.OpcUa;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application;

[ApplicationServiceCollectionGenerator]
public class PropertyReaderTest(ApplicationServiceCollectionFixture applicationServices)
{
    private readonly IPropertyReader _propertyReader = applicationServices.PropertyReader;
    
    [Test]
    [ReadablePropertyAttributesGenerator]
    [DisplayName($"Reading $fixture gives good status code")]
    public async Task CanReadProperty(PropertyAttributeFixture fixture)
    {
        var request = new ReadValueId()
        {
            AttributeId = fixture.AttributeId,
        };
        
        EntityValueReadResponse result = _propertyReader.ReadProperty(request, fixture.PropertyState);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(StatusCode.IsGood(result.ResponseCode)).IsTrue();
        await Assert.That(result.Response.DataValue.Value).IsNotDefault();
    }
}