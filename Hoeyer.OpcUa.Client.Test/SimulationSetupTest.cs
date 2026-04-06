using Hoeyer.Common.Extensions.Async;
using Hoeyer.OpcUa.Test.Adapter;
using Hoeyer.OpcUa.Test.Simulation;

namespace Hoeyer.OpcUa.Client.Test;

public sealed class SimulationSetupTest
{
    [Test]
    public async Task CanFindAdapter()
    {
        await Assert.That(FrameworkAdapter.AdapterInstance).IsNotNull();
    }

    [Test]
    [Timeout(50000)]
    [DependsOn(nameof(CanFindAdapter))]
    public async Task CanConnectToOpcuaServer(CancellationToken timeout)
    {
        var setup = new SimulationSetup(FrameworkAdapter.AdapterInstance!);
        var simulationServer = setup.Adapter.SimulationTarget;
        await simulationServer.InitializeAsync().WithCancellation(timeout);
        await Assert.That(await simulationServer.HealthCheck())
            .IsTrue();
    }
}