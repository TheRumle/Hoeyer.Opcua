using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;

namespace Hoeyer.OpcUa.IntegrationTest.Configuration;

[IntegrationAdapterDependentTest]
[DependsOn<ConfigurationCompatibilityTest>]
[IntegrationServiceInjection]
public class IntegrationEnvironmentHealthTests(
    EnvironmentHealthCheck healthCheck,
    IEntitySessionFactory sessionFactory)
{
    private const int CONNECTION_TIMEOUT = 5000;

    [Test]
    [Timeout(CONNECTION_TIMEOUT)]
    [DisplayName("The fixture must have a healthy environment")]
    public async Task TargetEnvironmentIsHealthy(CancellationToken timeout) =>
        await Assert.That(await healthCheck()).IsTrue();


    [Test]
    [DisplayName("Can connect to 1 session")]
    [Timeout(CONNECTION_TIMEOUT)]
    [DependsOn(nameof(TargetEnvironmentIsHealthy))]
    [NotInParallel(nameof(CanConnectToSessions))]
    public Task CanConnectTo1Session(CancellationToken timeout)
        => CanConnectToSessions(1, timeout);

    [Test]
    [DisplayName("Can connect to 2 sessions")]
    [Timeout(CONNECTION_TIMEOUT)]
    [DependsOn(nameof(CanConnectTo1Session))]
    [NotInParallel(nameof(CanConnectToSessions))]
    public Task CanConnectTo2Sessions(CancellationToken timeout)
        => CanConnectToSessions(2, timeout);

    [Test]
    [DisplayName("Can connect to 5 sessions")]
    [Timeout(CONNECTION_TIMEOUT)]
    [DependsOn(nameof(CanConnectTo2Sessions))]
    [NotInParallel(nameof(CanConnectToSessions))]
    public Task CanConnectTo5Sessions(CancellationToken timeout)
        => CanConnectToSessions(5, timeout);

    [Test]
    [DisplayName("Can connect to 10 sessions")]
    [Timeout(CONNECTION_TIMEOUT)]
    [DependsOn(nameof(CanConnectTo5Sessions))]
    [NotInParallel(nameof(CanConnectToSessions))]
    public Task CanConnectTo10Sessions(CancellationToken timeout)
        => CanConnectToSessions(10, timeout);

    private async Task CanConnectToSessions(
        int numberOfSessions,
        CancellationToken timeout)
    {
        var sessions = await Task.WhenAll(
            Enumerable.Range(0, numberOfSessions)
                .Select(_ =>
                {
                    var id = Guid.NewGuid().ToString();
                    return sessionFactory
                        .GetSessionAsync(id, timeout)
                        .SelectAsync(t => t.Session);
                })
                .ToArray());

        await Assert.That(sessions).All(session => session.Connected);
    }
}