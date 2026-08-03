using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;

public class ClientAndServerServicesAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private readonly ClientAndServerIntegrationServices _clientAndServerIntegrationServices = new();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        _clientAndServerIntegrationServices.Scope;

    public override object? Create(IServiceScope scope, Type type)
    {
        if (type == typeof(IServiceProvider)) return scope.ServiceProvider;
        return scope.ServiceProvider.GetRequiredService(type);
    }
}