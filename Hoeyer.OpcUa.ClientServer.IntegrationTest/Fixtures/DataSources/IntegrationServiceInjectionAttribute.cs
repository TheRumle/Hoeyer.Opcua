using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Microsoft.Extensions.DependencyInjection;
using static Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter.IntegrationTestAdapter;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;

/// <summary>
///     Uses the default integration test environment, dictated by key <see cref="GetSessionIsolatedAdapter" />.
///     The environment itself is session-owned and must not be disposed by this data source.
/// </summary>
public sealed class IntegrationServiceInjectionAttribute : AsyncUntypedDataSourceGeneratorAttribute
{
    private static IServiceProvider _singletonProvider = null!;

    private static readonly Task<IIntegrationTestEnvironment> TestEnvironment
        = InitializeTestEnvironmentAsync();

    private static async Task<IIntegrationTestEnvironment> InitializeTestEnvironmentAsync()
    {
        var environment = GetSessionIsolatedAdapter().TestEnvironment;
        await environment.InitializeAsync();
        _singletonProvider = environment.AvailableServices;
        return environment;
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
        var scope = _singletonProvider.CreateAsyncScope();
        dataGeneratorMetadata.TestBuilderContext.Current.Events.OnDispose += async (_, _) =>
        {
            await scope.DisposeAsync();
        };

        return dataGeneratorMetadata.MembersToGenerate
            .Select(GetMemberType)
            .Select(x => Create(scope, x))
            .ToArray();
    }

    private static object Create(IServiceScope scope, Type type)
        => scope.ServiceProvider.GetRequiredService(type);

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