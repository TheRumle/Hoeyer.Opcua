using Hoeyer.Common.Extensions.Async;
using Hoeyer.OpcUa.Test.Adapter;

namespace Hoeyer.OpcUa.Test.Client;

public class AdaptionOccuredAttribute()
    : SkipAttribute("There must be at least one assembly referrencing the test project for an adapter to be present")
{
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
    {
        return Task.FromResult(AppDomain.CurrentDomain
            .GetAssemblies()
            .Any(e => e.GetReferencedAssemblies().Contains(typeof(AdapterTest).Assembly.GetName())));
    }
}

public sealed class AdapterTest
{
    private static readonly ITestFrameworkAdapter Adapter = FrameworkAdapter.AdapterInstance;

    [Test]
    public async Task CanFindAdapter()
    {
        await Assert.That(Adapter).IsNotNull();
    }

    [Test]
    [Timeout(50000)]
    [DependsOn(nameof(CanFindAdapter))]
    public async Task CanConnectToOpcuaServer(CancellationToken timeout)
    {
        var simulationServer = Adapter.ConstructSimulationServer();
        await simulationServer.InitializeAsync().WithCancellation(timeout);
        await Assert.That(await simulationServer.HealthCheck())
            .IsTrue();
    }
}