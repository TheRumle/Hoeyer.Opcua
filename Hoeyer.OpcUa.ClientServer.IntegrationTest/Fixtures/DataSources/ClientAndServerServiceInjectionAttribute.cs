using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;

public sealed class ClientAndServerServiceInjectionAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    public readonly ServiceCollection Collection;
    private readonly ServiceProvider _provider;

    public ClientAndServerServiceInjectionAttribute()
    {
        var env = OpcEnvironment.Default(8000, "localhost");
        Collection = new ServiceCollection();
        _provider = Collection
            .AddClientAndServerTestServices(env, [typeof(TestEntity)]).Collection
            .BuildServiceProvider();
    }

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) => _provider.CreateScope();

    public override object? Create(IServiceScope scope, Type type)
    {
        var singleton = _provider.GetService(type);
        if (singleton != null) return singleton;
        return scope.ServiceProvider.GetRequiredService(type);
    }
}