using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Fixtures.Server;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Test;

public static class AssignServerAndServerServices
{
    public const int SERVER_FIXTURE_SERVER_REGISTRATION = 1;

    [Before(TestDiscovery, Order = SERVER_FIXTURE_SERVER_REGISTRATION)]
    public static void AssignInstance()
    {
        ServerFixtureSelector.AssignServerFixtureFactory(adapterKey =>
            new LocalHostedServerFixture((collection, opcEnvironment) =>
            {
                var testAssemblyMarker = typeof(IServerFixture);
                collection.AddSingleton<EnvironmentHealthCheck>(factory =>
                    {
                        return () => factory.GetRequiredService<IServerStartedHealthCheck>().ServerRunning();
                    })
                    .AddServerTestServices(opcEnvironment, [testAssemblyMarker]);
            }));
    }
}