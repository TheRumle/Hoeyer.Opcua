using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.Fixtures.Server;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.IntegrationTest;
using Playground.Application.EndToEndTest.Environment.Adapter.TestContainer;

namespace Playground.Application.EndToEndTest.Environment.Adapter;

public static class AssignTestContainerEnvironment
{
    [Before(TestDiscovery, Order = AssignLocallyHostedServer.LOCALHOST_DISCOVERY_ORDER + 1)]
    public static void AssignInstance()
    {
        ServerFixtureAdapter.AssignServerFixtureFactory(containerId => new PlaygroundTestContainer(WebProtocol.OpcTcp, containerId));
    }
}