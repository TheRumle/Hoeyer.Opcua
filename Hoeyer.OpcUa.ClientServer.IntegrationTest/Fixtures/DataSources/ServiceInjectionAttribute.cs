using Hoeyer.OpcUa.Fixtures.Common;
using Hoeyer.OpcUa.Fixtures.Common.Utils;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;

/// <summary>
///     This class does not start any <see cref="IServerFixture" /> and injects services.
///     from registered services of
///     <see
///         cref="ServiceCollectionExtensions.AddClientAndServerTestServices(Microsoft.Extensions.DependencyInjection.IServiceCollection,Hoeyer.OpcUa.Fixtures.Common.OpcEnvironment,System.Type[])" />
/// </summary>
public sealed class ServiceInjectionAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    public static readonly TestServiceCollection Services = new(ConfigureServices);

    private static void ConfigureServices(IServiceCollection collection)
    {
        collection.AddClientAndServerTestServices(OpcEnvironment.Default(8000, "localhost"),
            [typeof(DifferentFieldsEntity)]);
    }

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        Services.ServiceProvider.CreateScope();

    public override object Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetRequiredService(type);
}