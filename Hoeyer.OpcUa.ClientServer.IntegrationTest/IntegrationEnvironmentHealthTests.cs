using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.IntegrationTest.Attributes;
using Hoeyer.OpcUa.IntegrationTest.ConfigurationTest;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest;

[IntegrationDependentTest]
[DependsOn<ConfigurationCompatibilityTest>]
[ClassDataSource<IntegrationTestFixture>(Shared = SharedType.PerTestSession)]
public class IntegrationEnvironmentHealthTests(IntegrationTestFixture integrationTestFixture)
{
    [Test]
    [Timeout(10000)]
    [DisplayName("The fixture must have a healthy environment")]
    public async Task TargetEnvironmentIsHealthy(CancellationToken timeout) =>
        await Assert.That(await integrationTestFixture.ServerEnvironment.EnvironmentReady()).IsTrue();


    public static IEnumerable<TestDataRow<int>> NumberSupportedSessionsWanted()
    {
        int[] numbers = [1, 2, 5, 10];
        return numbers.Select(number => new TestDataRow<int>(
            DisplayName: $"The fixture must be able to provide {number} healthy, concurrent sessions",
            Data: number)
        );
    }

    [Test]
    [Timeout(10000)]
    [DependsOn(nameof(TargetEnvironmentIsHealthy))]
    [NotInParallel(nameof(CanConnectToSessions))]
    [MethodDataSource(nameof(NumberSupportedSessionsWanted))]
    public async Task CanConnectToSessions(int numberOfSessions, CancellationToken timeout)
    {
        var sessionFactory = integrationTestFixture.ServiceProvider.GetRequiredService<IEntitySessionFactory>();

        var sessions = await Task.WhenAll(Enumerable.Range(0, numberOfSessions)
            .Select(_ => sessionFactory.GetSessionAsync(Guid.NewGuid().ToString(), timeout).SelectAsync(t => t.Session))
            .ToArray());

        await Assert.That(sessions).All(session => session.Connected);
    }
}