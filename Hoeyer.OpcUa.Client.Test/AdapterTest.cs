using Hoeyer.Common.Extensions.Async;
using Hoeyer.OpcUa.Test.Adapter.Client;
using Hoeyer.OpcUa.Test.Adapter.Client.Api;

namespace OpcUa.Client.TestFramework;

public sealed class AdapterTest
{
    private static readonly Lazy<ITestFrameworkAdapter> Adapter = FrameworkAdapter.AdapterInstance;

    [Test]
    public async Task CanFindAdapter()
    {
        await Assert.That(Adapter.Value).IsNotNull();
    }

    [Test]
    [Timeout(50000)]
    [DependsOn(nameof(CanFindAdapter))]
    public async Task CanConnectToOpcuaServer(CancellationToken timeout)
    {
        var simulationServer = Adapter.Value.ConstructSimulationServer();
        await simulationServer.InitializeAsync().WithCancellation(timeout);
        await Assert.That(await simulationServer.HealthCheck())
            .IsTrue();
    }
}