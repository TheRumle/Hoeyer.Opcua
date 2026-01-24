using Hoeyer.OpcUa.Test.Simulation;

namespace Hoeyer.OpcUa.Test.Adapter.Client.Api;

public interface ITestFrameworkAdapter
{
    Type[] ClientAssemblyMarkers { get; }
    Type[] EntityAssemblyMarkers { get; }
    IOpcUaSimulationServer ConstructSimulationServer();
}