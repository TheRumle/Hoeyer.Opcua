using Hoeyer.OpcUa.Test.Adapter;

namespace Hoeyer.OpcUa.Test.Simulation.Scope;

internal class SimulationTestScopeFactory(ITestFrameworkAdapter adapter)
{
    public SimulationTestScope Keyed(string key) =>
        SimulationTestScope.Keyed(adapter, key);

    public SimulationTestScope PerTestSession() =>
        SimulationTestScope.PerTestSession(adapter);

    public SimulationTestScope PerClass() => SimulationTestScope.PerClass(adapter);
    public SimulationTestScope PerTest() => SimulationTestScope.PerTest(adapter);
}