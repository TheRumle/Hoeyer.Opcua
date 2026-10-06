using Hoeyer.OpcUa.Fixtures.Common;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;

public sealed class ClientAndServerServiceInjectionAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    public static readonly IServiceCollection Collection = new ServiceCollection()
        .AddClientAndServerTestServices(OpcEnvironment.Default(8000, "localhost"), [typeof(TestEntity)]).Collection;

    private static ServiceProvider _provider => Collection.BuildServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) => _provider.CreateScope();

    public override object? Create(IServiceScope scope, Type type)
    {
        var singleton = _provider.GetService(type);
        if (singleton != null) return singleton;
        return scope.ServiceProvider.GetRequiredService(type);
    }
}