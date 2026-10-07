using Hoeyer.OpcUa.Fixtures.Common;
using Hoeyer.OpcUa.Fixtures.Common.Utils;
using Hoeyer.OpcUa.Fixtures.Server.Entities;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Test;

public sealed class ServiceInjectionAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    public static readonly TestServiceCollection Services = new(ConfigureServices);

    private static void ConfigureServices(IServiceCollection collection)
    {
        var args = OpcEnvironment.Default(9999, "localhost");
        collection.AddServerTestServices(args, [typeof(AlarmTestEntity)]);
    }

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        Services.ServiceProvider.CreateScope();

    public override object Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetRequiredService(type);
}