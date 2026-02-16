using Hoeyer.OpcUa.Test.Adapter;
using Hoeyer.OpcUa.Test.Api;
using Hoeyer.OpcUa.Test.Simulation;

namespace Hoeyer.OpcUa.Test.Client;

public sealed class ClientTestFixture() : SimulationTestSession(
    new Lazy<SimulationSetup>(new SimulationSetup(
        FrameworkAdapter.AdapterInstance!.ConstructSimulationServer(),
        FrameworkAdapter.AdapterInstance!.EntityAssemblyMarkers.ToHashSet(),
        FrameworkAdapter.AdapterInstance!.ClientAssemblyMarkers.ToHashSet()
    )));