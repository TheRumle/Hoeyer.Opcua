using Hoeyer.OpcUa.Test.Adapter.Client.Api;
using Hoeyer.OpcUa.Test.Simulation;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace OpcUa.Client.TestFramework.ApplicationTest;

[ClassDataSource<AdaptedSimulationFixture>(Shared = SharedType.PerClass)]
[DependsOn<AdapterTest>]
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