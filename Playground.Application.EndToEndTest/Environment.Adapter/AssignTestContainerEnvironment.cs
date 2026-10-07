using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.Server.Test;
using Playground.Application.EndToEndTest.Environment.Adapter.TestContainer;

namespace Playground.Application.EndToEndTest.Environment.Adapter;

public static class AssignTestContainerEnvironment
{
    [Before(TestDiscovery, Order = AssignServerAndServerServices.SERVER_FIXTURE_SERVER_REGISTRATION + 1)]
    public static void AssignInstance()
    {
        ServerFixtureSelector.AssignServerFixtureFactory(containerId =>
            new PlaygroundTestContainer(WebProtocol.OpcTcp, containerId));
    }
}