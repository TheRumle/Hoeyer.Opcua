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


    public async ValueTask DisposeAsync()
    {
        await ServerEnvironment.DisposeAsync();
        if (ServiceProvider is IAsyncDisposable serviceScopeAsyncDisposable)
        {
            await serviceScopeAsyncDisposable.DisposeAsync();
        }
        else if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
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