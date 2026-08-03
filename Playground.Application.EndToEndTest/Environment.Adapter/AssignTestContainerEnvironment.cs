using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter.LocallyHostedServer;
using Microsoft.Extensions.DependencyInjection;
using Playground.Application.EndToEndTest.Environment.Adapter.Docker;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest.Environment.Adapter;

public static class AssignTestContainerEnvironment
{
    [Before(TestDiscovery, Order = AssignLocallyHostedEnvironment.LOCALHOST_DISCOVERY_ORDER + 1)]
    public static void AssignInstance()
    {
        IntegrationTestAdapter.AssignFuncFactory(s => new DockerEnvironmentAdapter(s));
    }

    private sealed class DockerEnvironmentAdapter(string containerId) : IIntegrationTestEnvironmentAdapter
    {
        public IIntegrationTestEnvironment TestEnvironment { get; } =
            new PlaygroundTestContainer(WebProtocol.OpcTcp, containerId);

        public IServiceCollection ApplicationServices => new ServiceCollection();

        public Type[] ClientAssemblyMarkers => [typeof(Gantry)];
        public Type[] EntityAssemblyMarkers => [typeof(IGantryMethods)];
    }
}