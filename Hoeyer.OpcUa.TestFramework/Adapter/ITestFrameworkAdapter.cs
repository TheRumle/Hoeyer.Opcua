namespace Hoeyer.OpcUa.Test.Adapter;

public interface ITestFrameworkAdapter
{
    Type[] ClientAssemblyMarkers { get; }
    Type[] EntityAssemblyMarkers { get; }
    IOpcUaSimulationServer ConstructSimulationServer();
}