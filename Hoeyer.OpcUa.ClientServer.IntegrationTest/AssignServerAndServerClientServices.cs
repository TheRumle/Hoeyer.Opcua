using Hoeyer.OpcUa.Core.Configuration.Health;
using Hoeyer.OpcUa.Fixtures.Server;
using Hoeyer.OpcUa.Fixtures.Server.Entities;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Hoeyer.OpcUa.Server.Test;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest;

public static class AssignServerAndServerClientServices
{
    [Before(TestDiscovery, Order = AssignServerAndServerServices.SERVER_FIXTURE_SERVER_REGISTRATION + 1)]
    public static void AssignInstance()
    {
        ServerFixtureSelector.AssignServerFixtureFactory(adapterKey =>
            new LocalHostedServerFixture((collection, opcEnvironment) =>
            {
                Type[] testAssemblyMarker = [typeof(DifferentFieldsEntity), typeof(AlarmTestEntity)];
                collection.AddSingleton<EnvironmentHealthCheck>(factory =>
                    {
                        return () => factory.GetRequiredService<IServerStartedHealthCheck>().ServerRunning();
                    })
                    .AddClientAndServerTestServices(opcEnvironment, testAssemblyMarker);
            }));
    }
}