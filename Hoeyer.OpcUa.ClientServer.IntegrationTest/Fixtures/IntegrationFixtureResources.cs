using Hoeyer.OpcUa.Fixtures.Server.FixtureKeys;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

public interface IIntegrationFixtureResources : IAsyncInitializer
{
    IServiceProvider ServiceProvider { get; }
}

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
internal sealed class IntegrationFixtureResources<T>(Func<string, IServerFixture> adapterProvider) : IIntegrationFixtureResources
{
    private IServerFixture _environment;
    public IServiceProvider ServiceProvider { get; private set; }

    public async Task InitializeAsync()
    {
        var key = TestContextExtensions.ComputeKey<T>(TestContext.Current!);
        var fixture = adapterProvider(key);

        _environment = fixture;
        await _environment.InitializeAsync();
        ServiceProvider = _environment.AvailableServices;
    }
}