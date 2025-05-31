using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using Hoeyer.opcUa.TestEntities.Methods;

namespace Hoeyer.OpcUa.EndToEndTest;

[GeneratedClassTest]
[ApplicationFixtureGenerator<IGantryMethods>]
public class MethodCallingTest(ApplicationFixture<IGantryMethods> appFixture)
{
    [Test]
    public async Task WhenCallingVoidTask_DoesNotThrow() => await appFixture.TestedService.A(2);

    [Test]
    public async Task WhenArgMethods_DoesNotThrow() => await appFixture.TestedService.LetsGoInt(2, 21.0f, [1, 23, 3]);

    [Test]
    public async Task WhenArgMethods_DoesNotThrow_More()
    {
        await appFixture.TestedService.MoreMethods(2, 21.0f, 2f, []);
    }
}