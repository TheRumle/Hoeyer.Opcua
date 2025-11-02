using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

[ApplicationFixtureGenerator<IGantryMethods>]
public class MethodCallingTest(ApplicationFixture<IGantryMethods> appFixture)
{
    [Test]
    public async Task WhenCallingVoidTask_DoesNotThrow()
        => await appFixture.TestedService.ChangePosition(Position.OnTheMoon);


    [Test]
    public async Task<int> WhenCalling_TaskWithGuidReturn_DoesNotThrow() =>
        await appFixture.TestedService.PlaceContainer(Position.OnTheMoon);
}