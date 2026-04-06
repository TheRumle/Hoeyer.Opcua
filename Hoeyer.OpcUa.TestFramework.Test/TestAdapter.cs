using Hoeyer.OpcUa.Test.Adapter;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.TestFramework.Test;

public class TestAdapter : ITestFrameworkAdapter
{
    public Type[] ClientAssemblyMarkers { get; } = [];
    public Type[] EntityAssemblyMarkers { get; } = [];
    public IOpcUaSimulationTarget SimulationTarget { get; } = new TestSimulationTargetConfig();
    public IServiceCollection ApplicationServices { get; } = new ServiceCollection();
}