using Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

[ClassDataSource<SimulationFixture>(Shared = SharedType.PerTestSession)]
public class MethodCallingTest(SimulationFixture fixture)
{
    private readonly Lazy<IGantryMethods> _gantryMethods = new(fixture.GetService<IGantryMethods>);

    [Test]
    public async Task WhenCallingVoidTask_DoesNotThrow()
        => await _gantryMethods.Value.ChangePosition(Position.OnTheMoon);


    [Test]
    public async Task WhenCalling_TaskWithGuidReturn_DoesNotThrow() =>
        await _gantryMethods.Value.AssignContainer(Guid.NewGuid());
}