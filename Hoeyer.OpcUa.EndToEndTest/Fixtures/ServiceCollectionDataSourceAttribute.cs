using Hoeyer.OpcUa.EntityModelling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class ServiceCollectionDataSourceAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider ServiceProvider = CreateSharedServiceProvider();

    public IEnumerable<ServiceDescriptor> Services => CreateServiceCollection();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
        => ServiceProvider.CreateAsyncScope();

    public override object Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type)!;

    private static IServiceProvider CreateSharedServiceProvider() => CreateServiceCollection().BuildServiceProvider();

    private static IServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection()
            .AddTestEntityServices(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithHttpsHost("localhost", 5)
                .WithEndpoints(["opc.tcp://localhost:5"])
                .Build())
            .AddLogging(e => e.AddSimpleConsole());


        services.AddSingleton(services);
        services.AddScoped<IServiceProvider>(p => p);
        return services;
    }

    public static implicit operator List<ServiceDescriptor>(
        ServiceCollectionDataSourceAttribute servicesServerFixtureAttribute) =>
        servicesServerFixtureAttribute.Services.ToList();
}