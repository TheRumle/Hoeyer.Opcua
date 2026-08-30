using Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;

namespace Hoeyer.OpcUa.IntegrationTest.Configuration;

[IntegrationServiceInjection]
public sealed class HealthyOpcUaServer(
    EnvironmentHealthCheck healthCheck
)
{
    private const int CONNECTION_TIMEOUT = 5000;

    [Test]
    [Timeout(CONNECTION_TIMEOUT)]
    [DisplayName("The fixture must have a healthy environment")]
    public async Task TargetEnvironmentIsHealthy(CancellationToken timeout) =>
        await Assert.That(await healthCheck()).IsTrue();
}