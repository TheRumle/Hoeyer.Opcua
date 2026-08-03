using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

internal sealed class IntegrationFixtureResources<T>(Func<string, IIntegrationTestEnvironmentAdapter> adapterProvider)
    : IAsyncInitializer, IDisposable
{
    internal IServiceProvider ServiceProvider { get; private set; } = null!;
    internal IIntegrationTestEnvironment ServerEnvironment { get; private set; } = null!;
    private IServiceScope ServiceScope { get; set; } = null!;

    public async Task InitializeAsync()
    {
        var context = TestContext.Current!;
        var key = TestContextExtensions.ComputeKey<T>(context);
        var adapter = adapterProvider(key);
        ServerEnvironment = adapter.TestEnvironment;

        await ServerEnvironment.InitializeAsync();

        var adapterArgs = new ClientServicesAdapterArgs
        {
            HostName = ServerEnvironment.Host,
            Port = ServerEnvironment.SimulationPort,
            OpcUaServerId = ServerEnvironment.ServerId,
            OpcUaServerName = ServerEnvironment.ServerName,
            Protocol = ServerEnvironment.Protocol
        };

        var exposedServices = new ClientIntegrationServices(
            adapter.ApplicationServices, adapterArgs,
            adapter.EntityAssemblyMarkers,
            adapter.ClientAssemblyMarkers
        );

        ServiceScope = exposedServices.ServiceProvider.CreateScope();
        ServiceProvider = ServiceScope.ServiceProvider;
    }


    public void Dispose() => ServiceScope?.Dispose();
}