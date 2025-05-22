using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using Hoeyer.OpcUa.EndToEndTest.TestEntities.Methods;

namespace Hoeyer.OpcUa.EndToEndTest;

[GeneratedClassTest]
[ApplicationFixtureGenerator<IGantryMethods>]
public class MethodCallingTest(ApplicationFixture<IGantryMethods> appFixture)
{
    [Test]
    public async Task WhenCallingVoidTask_DoesNotThrow() => await appFixture.TestedService.A(2);
}