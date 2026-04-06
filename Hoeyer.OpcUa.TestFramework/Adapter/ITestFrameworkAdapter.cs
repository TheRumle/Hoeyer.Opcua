using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Test.Adapter;

public interface ITestFrameworkAdapter
{
    Type[] ClientAssemblyMarkers { get; }
    Type[] EntityAssemblyMarkers { get; }
    IOpcUaSimulationTarget SimulationTarget { get; }
    IServiceCollection ApplicationServices { get; }
}