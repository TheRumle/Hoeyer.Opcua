using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.IntegrationTest.Configuration;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest.MethodCalling;

[DependsOn<SessionConnectionTest>]
[ClassDataSource<RuntimeSelectedServer>]
public class MethodCallingTest(RuntimeSelectedServer fixture)
{
    [Test]
    public async Task WhenCallingVoidTask_DoesNotThrow()
        => await fixture.GetService<IGantryMethods>().ChangePosition(Position.OnTheMoon);


    [Test]
    public async Task WhenCalling_TaskWithGuidReturn_DoesNotThrow() =>
        await fixture.GetService<IGantryMethods>().AssignContainer(Guid.NewGuid());
}