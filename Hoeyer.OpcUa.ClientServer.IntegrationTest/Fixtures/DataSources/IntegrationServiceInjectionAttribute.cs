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
    private static readonly Task<IIntegrationTestEnvironment> TestEnvironment
        = InitializeTestEnvironmentAsync();

    protected override async IAsyncEnumerable<Func<Task<object?[]?>>> GenerateDataSourcesAsync(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        yield return () => CreateDataUsingScope(dataGeneratorMetadata);

        await Task.CompletedTask;
    }


    private static async Task<IIntegrationTestEnvironment> InitializeTestEnvironmentAsync()
    {
        var environment = GetSessionIsolatedAdapter().TestEnvironment;
        await environment.InitializeAsync().ConfigureAwait(false);
        return environment;
    }

    private static async Task<object?[]?> CreateDataUsingScope(DataGeneratorMetadata dataGeneratorMetadata)
    {
        var testEnvironment = await TestEnvironment;
        var scopeContainer = new ScopeContainer(() => testEnvironment.AvailableServices
            .BuildServiceProvider()
            .CreateScope());

        dataGeneratorMetadata.TestBuilderContext.Current.Events.OnDispose += async (_, _) =>
        {
            switch (scopeContainer.Scope)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        };

        return dataGeneratorMetadata.MembersToGenerate
            .Select(GetMemberType)
            .Select(x => Create(scopeContainer.Scope, x))
            .ToArray();
    }

    private static object Create(IServiceScope scope, Type type)
    {
        if (type == typeof(IServiceProvider))
        {
            return scope.ServiceProvider;
        }

        return scope.ServiceProvider.GetRequiredService(type);
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

    private sealed class ScopeContainer(Func<IServiceScope> scopeFactory)
    {
        private readonly Lazy<IServiceScope> _scope = new(scopeFactory);
        public IServiceScope Scope => _scope.Value;
    }
}