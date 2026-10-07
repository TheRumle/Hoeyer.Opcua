using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Fixtures.Server.FixtureKeys;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

/// <summary>
/// Injecting this fixture using <see cref="ClassDataSourceAttribute{T}"/>
/// ensures that underlying an <see cref="IServerFixture"/> is up-and-running. Services created using this Fixture will be configured to connect to the underlying server of the <see cref="IServerFixture"/>.
/// Injecting using the different keys can result in new <see cref="IServerFixture"/> being started.
/// <list type="bullet">
///         <item>
///             <description>
///                 Using <see cref="SharedType.PerTestSession"/> will result in getting the same <see cref="IServerFixture"/> as if
///                 using <see cref="Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources.IntegrationServiceInjectionAttribute"/>
///                 or calling <see cref="ServerFixtureAdapter.GetSessionSharedServerFixture"/> with key <see cref="TestKeys.PerTestSessionKey"/>.
///                 Tests injecting in this manner should not modify the state of the <see cref="IServerFixture"/>.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Using <see cref="SharedType.PerAssembly"/> will result in a shared per-assembly <see cref="IServerFixture"/>.
///                 Tests injecting in this manner should not modify the state of the <see cref="IServerFixture"/>.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Using <see cref="SharedType.Keyed"/> will result in a shared per-key <see cref="IServerFixture"/>.
///                 Tests injecting in this manner are allowed to modify the state of the <see cref="IServerFixture"/>.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Using <see cref="SharedType.None"/> will result in a shared  <see cref="IServerFixture"/> per test.
///                 This should be avoided as setting up an <see cref="IServerFixture"/> is expensive.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Using <see cref="SharedType.PerClass"/> will result in a <see cref="IServerFixture"/> being shared across a test class.
///                 This is an appropriate way of writing scenario tests. 
///             </description>
///         </item>
///     </list>
/// </summary>
public class IsolatedServerFixture
{
    private readonly string _id = Guid.NewGuid().ToString();

    protected virtual IIntegrationFixtureResources Resources { get; } =
        new IntegrationFixtureResources<IServerFixture>(ServerFixtureAdapter.CreateOrGetCached);

    public IServiceProvider ServiceProvider => Resources.ServiceProvider;


    public TWanted GetService<TWanted>() where TWanted : notnull => ServiceProvider.GetService<TWanted>()!;

    public async Task<IEntitySession> OpenSession() => await ServiceProvider
        .GetRequiredService<IEntitySessionFactory>().GetSessionAsync(_id);
}

/// <inheritdoc/>
/// <typeparam name="T">The service being tested</typeparam>
public sealed class IsolatedServerFixture<T> : IsolatedServerFixture
    where T : notnull
{
    protected override IIntegrationFixtureResources Resources { get; }
        = new IntegrationFixtureResources<IsolatedServerFixture<T>>(ServerFixtureAdapter.CreateOrGetCached);

    public T TestedService => GetService<T>();
}