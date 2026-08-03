using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter.LocallyHostedServer;

internal sealed class LocalHostedIntegrationTestEnvironmentAdapter : IIntegrationTestEnvironmentAdapter
{
    private ClientAndServerIntegrationServices _services = new();

    public LocalHostedIntegrationTestEnvironmentAdapter()
    {
        TestEnvironment = new LocalHostedIntegrationTestEnvironment(_services);
    }

    public Type[] ClientAssemblyMarkers { get; } = [typeof(TestAssemblyMarker)];
    public Type[] EntityAssemblyMarkers { get; } = [typeof(TestAssemblyMarker)];
    public IIntegrationTestEnvironment TestEnvironment { get; }

    public IServiceCollection ApplicationServices { get; } =
        new ClientAndServerIntegrationServices().ServiceCollection;
}