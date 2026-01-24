using Hoeyer.OpcUa.Test.Simulation;

namespace Hoeyer.OpcUa.Test.Adapter.Client.Api;

public sealed class AdaptedSharedSimulationServiceContext<T>() : SharedSimulationServiceContext<T>(new SimulationSetup(
    adapter.ConstructSimulationServer(),
    adapter.EntityAssemblyMarkers.ToHashSet(),
    adapter.ClientAssemblyMarkers.ToHashSet()
))
{
    private static readonly ITestFrameworkAdapter adapter = FrameworkAdapter.AdapterInstance.Value;

    public static implicit operator List<ISpecifiedTestSession<T>>(AdaptedSharedSimulationServiceContext<T> provider) =>
        provider.GetSpecifiedSessions();
}