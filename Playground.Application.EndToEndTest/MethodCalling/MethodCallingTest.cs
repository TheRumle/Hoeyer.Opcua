using Hoeyer.OpcUa.IntegrationTest;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest.MethodCalling;

[DependsOn<IntegrationEnvironmentHealthTests>]
[ClassDataSource<IntegrationTestFixture<IGantryMethods>>]
public class MethodCallingTest(IntegrationTestFixture<IGantryMethods> methods)
{
    [Test]
    public async Task WhenCallingVoidTask_DoesNotThrow()
        => await methods.TestedService.ChangePosition(Position.OnTheMoon);


    [Test]
    public async Task WhenCalling_TaskWithGuidReturn_DoesNotThrow() =>
        await methods.TestedService.AssignContainer(Guid.NewGuid());
}