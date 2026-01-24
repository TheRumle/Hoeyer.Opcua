using Hoeyer.OpcUa.Test.Simulation;

namespace Hoeyer.OpcUa.Test.Adapter.Client.Api;

public sealed class AdaptedSimulationFixture() : SimulationTestSession(new SimulationSetup(
    adapter.ConstructSimulationServer(),
    adapter.EntityAssemblyMarkers.ToHashSet(),
    adapter.ClientAssemblyMarkers.ToHashSet()
))
{
    private static readonly ITestFrameworkAdapter adapter = FrameworkAdapter.AdapterInstance.Value;
}