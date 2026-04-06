using Hoeyer.OpcUa.Client.Test;
using Hoeyer.OpcUa.Test.Api;
using Hoeyer.OpcUa.Test.Api.Attributes;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest;

[DependsOn<SimulationSetupTest>]
[PerClassSimulationFixture]
public class MethodCallingTest(ISimulationTestSession fixture)
{
    private readonly Lazy<IGantryMethods> _gantryMethods = new(fixture.GetService<IGantryMethods>);

    [Test]
    public async Task WhenCallingVoidTask_DoesNotThrow()
        => await _gantryMethods.Value.ChangePosition(Position.OnTheMoon);


    [Test]
    public async Task WhenCalling_TaskWithGuidReturn_DoesNotThrow() =>
        await _gantryMethods.Value.AssignContainer(Guid.NewGuid());
}