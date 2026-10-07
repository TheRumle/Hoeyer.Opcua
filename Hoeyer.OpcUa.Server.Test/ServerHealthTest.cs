using Hoeyer.OpcUa.Fixtures.Server;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;

namespace Hoeyer.OpcUa.Server.Test;

[ClassDataSource<RuntimeSelectedServer>(Shared = SharedType.PerTestSession)]
public sealed class ServerHealthTest(
    RuntimeSelectedServer server
)
{
    private const int CONNECTION_TIMEOUT = 5000;

    [Test]
    [Timeout(CONNECTION_TIMEOUT)]
    [DisplayName("The fixture must have a healthy environment")]
    public async Task TargetEnvironmentIsHealthy(CancellationToken timeout) =>
        await Assert.That(await server.GetService<EnvironmentHealthCheck>().Invoke()).IsTrue();
}