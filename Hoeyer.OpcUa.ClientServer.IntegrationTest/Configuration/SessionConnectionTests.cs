using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.IntegrationTest.Extensions;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;
using Hoeyer.OpcUa.Server.Abstractions;

namespace Hoeyer.OpcUa.IntegrationTest.ConfigurationTest;

[ClientAndServerServices]
public class SessionConnectionTests(
    IStartableEntityServer entityServer,
    IEntitySessionFactory sessionFactory)
{
    [Test]
    [DisplayName("When server is started, session factory can create a session to the server")]
    public async Task WhenServerStarted_CanConnectSession(CancellationToken cancellationToken = default)
    {
        await entityServer.StartAsync();
        await sessionFactory.GetSessionAsync(TestKeys.PER_TEST_SESSION_KEY, cancellationToken);
    }
}