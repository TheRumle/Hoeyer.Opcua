using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.Extensions;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

internal sealed class IntegrationFixtureResources<T>(Func<string, IIntegrationTestEnvironmentAdapter> adapterProvider)
    : IAsyncInitializer, IAsyncDisposable
{
    internal IntegrationTestServiceProvider _integrationTestServiceProvider;
    internal IServiceProvider ServiceProvider => _integrationTestServiceProvider.SingletonProvider;
    internal IIntegrationTestEnvironment? ServerEnvironment { get; private set; } = null!;


    public async ValueTask DisposeAsync()
    {
        if (ServerEnvironment is not null) await ServerEnvironment.DisposeAsync();
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
        _integrationTestServiceProvider = ServerEnvironment.AvailableServices;
    }
}