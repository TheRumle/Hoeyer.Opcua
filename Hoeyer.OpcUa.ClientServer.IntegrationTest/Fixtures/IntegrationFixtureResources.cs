using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.Extensions;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

/// <summary>
///     Borrows the session-owned integration test environment for the duration of a test class.
///     <para>
///         This type owns no disposable state. The adapter, the environment and its root
///         <see cref="IServiceProvider" /> are cached in <see cref="IntegrationTestAdapter" /> and shared by every
///         test class in the session, and the session disposes them once at the end via
///         <c>DisposeCachedEnvironments</c>. TUnit disposes this object when the class finishes, so disposing the
///         session state here would tear the shared environment down for every class that runs afterwards.
///     </para>
/// </summary>
internal sealed class IntegrationFixtureResources<T>(Func<string, IIntegrationTestEnvironmentAdapter> adapterProvider)
    : IAsyncInitializer, IAsyncDisposable
{
    private IntegrationTestServiceProvider? _integrationTestServiceProvider;
    private IIntegrationTestEnvironment? _serverEnvironment;

    internal IServiceProvider ServiceProvider => InitializedServiceProvider?.SingletonProvider!;

    internal IIntegrationTestEnvironment ServerEnvironment => _serverEnvironment!;

    private IntegrationTestServiceProvider? InitializedServiceProvider => _integrationTestServiceProvider;

    /// <summary>
    ///     Intentionally does nothing. The environment and its service provider are session-owned; see the remarks on
    ///     <see cref="IntegrationFixtureResources{T}" />.
    /// </summary>
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public async Task InitializeAsync()
    {
        var key = TestContextExtensions.ComputeKey<T>(TestContext.Current!);
        var adapter = adapterProvider(key);

        var environment = adapter.TestEnvironment;
        await environment.InitializeAsync();
        _serverEnvironment = environment;
        _integrationTestServiceProvider = environment.AvailableServices;
    }
}