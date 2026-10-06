using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Fixtures.Server;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest;

public static class AssignLocallyHostedServer
{
    public const int LOCALHOST_DISCOVERY_ORDER = 1;

    [Before(TestDiscovery, Order = LOCALHOST_DISCOVERY_ORDER)]
    public static void AssignInstance()
    {
        ServerFixtureAdapter.AssignServerFixtureFactory(adapterKey => new LocalHostedServerFixture((collection, opcEnvironment) =>
        {
            var testAssemblyMarker = typeof(TestEntity);
            collection.AddSingleton<EnvironmentHealthCheck>(factory =>
                {
                    return () => factory.GetRequiredService<IServerStartedHealthCheck>().ServerRunning();
                })
                .AddClientAndServerTestServices(opcEnvironment, [testAssemblyMarker]);
        }));
    }
}