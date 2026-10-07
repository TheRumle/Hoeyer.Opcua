using Hoeyer.OpcUa.Fixtures.Common.Utils;
using Hoeyer.OpcUa.Fixtures.Server.FixtureKeys;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Fixtures.Server.ServerFixture;

/// <summary>
///     Wraps an <see cref="IServerFixture" /> assigned on startup using
///     Injecting this fixture using <see cref="ClassDataSourceAttribute{T}" />
///     ensures that underlying an <see cref="IServerFixture" /> is up-and-running. Services created using this Fixture
///     will be configured to connect to the underlying server of the <see cref="IServerFixture" />.
///     Injecting using the different keys can result in new <see cref="IServerFixture" /> being started.
///     <list type="bullet">
///         <item>
///             <description>
///                 Using <see cref="SharedType.PerTestSession" /> will result in getting a globally shared
///                 <see cref="IServerFixture" />.
///                 Tests injecting in this manner should not modify the state of the <see cref="IServerFixture" />.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Using <see cref="SharedType.PerAssembly" /> will result in a shared per-assembly
///                 <see cref="IServerFixture" />.
///                 Tests injecting in this manner should not modify the state of the <see cref="IServerFixture" />.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Using <see cref="SharedType.Keyed" /> will result in a shared per-key <see cref="IServerFixture" />.
///                 Tests injecting in this manner are allowed to modify the state of the <see cref="IServerFixture" />.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Using <see cref="SharedType.PerClass" /> will result in a <see cref="IServerFixture" /> being shared
///                 across a test class.
///                 This is an appropriate way of writing scenario tests.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Using <see cref="SharedType.None" /> will result in a shared  <see cref="IServerFixture" /> per test.
///                 This should be avoided as setting up an <see cref="IServerFixture" /> is expensive.
///             </description>
///         </item>
///     </list>
/// </summary>
public class RuntimeSelectedServer : IAsyncInitializer, IAsyncDisposable
{
    private IServerFixture _environment;
    public IServiceProvider ServiceProvider { get; set; }

    public IServiceProvider AvailableServices => _environment.AvailableServices;
    public ValueTask DisposeAsync() => _environment.DisposeAsync();

    public async Task InitializeAsync()
    {
        var key = TestContextExtensions.ComputeKey<RuntimeSelectedServer>(TestContext.Current!);
        var fixture = ServerFixtureSelector.CreateOrGetCached(key);

        _environment = fixture;
        await _environment.InitializeAsync();
        ServiceProvider = _environment.AvailableServices;
    }

    public TWanted GetService<TWanted>() where TWanted : notnull => ServiceProvider.GetRequiredService<TWanted>();

    public NonOwned<TWanted> GetNonOwned<TWanted>() where TWanted : class =>
        ServiceProvider.GetService<NonOwned<TWanted>>()!;
}