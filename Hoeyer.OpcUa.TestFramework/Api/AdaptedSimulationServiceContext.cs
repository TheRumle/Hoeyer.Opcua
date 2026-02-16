using Hoeyer.OpcUa.Test.Adapter;
using Hoeyer.OpcUa.Test.Simulation;

namespace Hoeyer.OpcUa.Test.Api;

public sealed class AdaptedSimulationServiceContext<T>() : SimulationServiceContext<T>(new Lazy<SimulationSetup>(() =>
    new SimulationSetup(
        FrameworkAdapter.AdapterInstance.ConstructSimulationServer(),
        FrameworkAdapter.AdapterInstance.EntityAssemblyMarkers.ToHashSet(),
        FrameworkAdapter.AdapterInstance.ClientAssemblyMarkers.ToHashSet()
    )))
{
    public static implicit operator List<ISpecifiedTestSession<T>>(AdaptedSimulationServiceContext<T> context) =>
        context.GetSpecifiedSessions();
}