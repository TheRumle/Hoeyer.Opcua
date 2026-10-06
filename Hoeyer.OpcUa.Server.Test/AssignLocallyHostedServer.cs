using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Fixtures.Server;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.Server.Test.Fixtures.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Test;

public static class AssignLocallyHostedServer
{
    public const int LOCALHOST_DISCOVERY_ORDER = 1;

    [Before(TestDiscovery, Order = 2)]
    public static void AssignInstance()
    {
        ServerFixtureAdapter.AssignServerFixtureFactory(adapterKey => new LocalHostedServerFixture((collection, opcEnvironment) =>
        {
            var testAssemblyMarker = typeof(AlarmTestEntity);
            collection.AddSingleton<EnvironmentHealthCheck>(factory =>
                {
                    return () => factory.GetRequiredService<IServerStartedHealthCheck>().ServerRunning();
                })
                .AddServerTestServices(opcEnvironment, [testAssemblyMarker]);
        }));
    }
}