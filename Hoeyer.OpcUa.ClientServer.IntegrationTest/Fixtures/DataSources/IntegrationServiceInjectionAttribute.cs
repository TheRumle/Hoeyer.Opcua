using Hoeyer.OpcUa.Fixtures.Server.FixtureKeys;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Microsoft.Extensions.DependencyInjection;
using static Hoeyer.OpcUa.Fixtures.Server.ServerFixture.ServerFixtureAdapter;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;

/// <summary>
///     Uses the default <see cref="IServerFixture"/> environment and creates services that connect to the fixture.
///     The environment itself is session-owned and must not be disposed by this data source.
///     The underlying <see cref="IServerFixture"/> will be the same as when calling <see cref="ServerFixtureAdapter.GetSessionSharedServerFixture"/> with key <see cref="TestKeys.PerTestSessionKey"/>.
/// </summary>
public sealed class IntegrationServiceInjectionAttribute : AsyncUntypedDataSourceGeneratorAttribute
{
    private static IServiceProvider _provider = null!;

    private static readonly Task<IServerFixture> TestEnvironment
        = InitializeTestEnvironmentAsync();

    private static async Task<IServerFixture> InitializeTestEnvironmentAsync()
    {
        var serverFixture = GetSessionSharedServerFixture();
        await serverFixture.InitializeAsync();
        _provider = serverFixture.AvailableServices;
        return serverFixture;
    }

    protected override async IAsyncEnumerable<Func<Task<object?[]?>>> GenerateDataSourcesAsync(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        yield return () => CreateDataUsingScope(dataGeneratorMetadata);
        await Task.CompletedTask;
    }

    private static async Task<object?[]?> CreateDataUsingScope(DataGeneratorMetadata dataGeneratorMetadata)
    {
        await TestEnvironment;
        var scope = _provider.CreateAsyncScope();
        dataGeneratorMetadata.TestBuilderContext.Current.Events.OnDispose += async (_, _) =>
        {
            await scope.DisposeAsync();
        };

        return dataGeneratorMetadata.MembersToGenerate
            .Select(GetMemberType)
            .Select(x => scope.ServiceProvider.GetService(x))
            .ToArray();
    }

    private static Type GetMemberType(IMemberMetadata member) =>
        member switch
        {
            PropertyMetadata prop => prop.Type,
            ParameterMetadata param => param.Type,
            ClassMetadata cls => cls.Type,
            MethodMetadata method => method.Type,
            var _ => throw new InvalidOperationException(
                $"Unknown member type: {member.GetType()}")
        };
}