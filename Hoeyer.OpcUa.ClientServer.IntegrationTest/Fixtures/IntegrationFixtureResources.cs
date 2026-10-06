using Hoeyer.OpcUa.Fixtures.Server;
using Hoeyer.OpcUa.Fixtures.Server.FixtureKeys;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

/// <summary>
///     Borrows the session-owned integration test environment for the duration of a test class.
///     <para>
///         This type owns no disposable state. The adapter, the environment and its root
///         <see cref="IServiceProvider" /> are cached in <see cref="ServerFixtureAdapter" /> and shared by every
///         test class in the session, and the session disposes them once at the end via
///         <c>DisposeCachedEnvironments</c>. TUnit disposes this object when the class finishes, so disposing the
///         session state here would tear the shared environment down for every class that runs afterwards.
///     </para>
/// </summary>
internal sealed class IntegrationFixtureResources<T>(Func<string, IServerFixture> adapterProvider)
    : IAsyncInitializer
{
    private IServerFixture? _serverEnvironment;
    internal IServiceProvider ServiceProvider { get; private set; }

    internal IServerFixture ServerEnvironment => _serverEnvironment!;

    public async Task InitializeAsync()
    {
        var key = TestContextExtensions.ComputeKey<T>(TestContext.Current!);
        var fixture = adapterProvider(key);

        var environment = fixture;
        await environment.InitializeAsync();
        _serverEnvironment = environment;
        ServiceProvider = environment.AvailableServices;
    }
}