using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

internal sealed class IntegrationFixtureResources<T>(Func<string, IIntegrationTestEnvironmentAdapter> adapterProvider)
    : IAsyncInitializer, IAsyncDisposable
{
    internal IServiceProvider ServiceProvider { get; private set; } = null!;
    internal IIntegrationTestEnvironment ServerEnvironment { get; private set; } = null!;
    private IServiceScope ServiceScope { get; } = null!;


    public async ValueTask DisposeAsync()
    {
        await ServerEnvironment.DisposeAsync();
        if (ServiceScope is IAsyncDisposable serviceScopeAsyncDisposable)
        {
            await serviceScopeAsyncDisposable.DisposeAsync();
        }
        else
        {
            ServiceScope.Dispose();
        }
    }

    public async Task InitializeAsync()
    {
        var key = TestContextExtensions.ComputeKey<T>(TestContext.Current!);
        var adapter = adapterProvider(key);

        ServerEnvironment = adapter.TestEnvironment;
        await ServerEnvironment.InitializeAsync();

        ServiceProvider = ServerEnvironment
            .AvailableServices
            .CreateScope()
            .ServiceProvider;
    }
}