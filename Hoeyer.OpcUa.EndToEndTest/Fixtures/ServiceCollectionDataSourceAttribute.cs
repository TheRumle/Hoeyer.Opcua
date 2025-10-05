using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
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
        var services = new ServiceCollection().AddOpcUa(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithWebOrigins(WebProtocol.Https, "localhost", 5)
                .Build())
            .WithEntityServices()
            .WithOpcUaClientServices()
            .WithOpcUaServer()
            .WithOpcUaSimulationServices(c => c.AdaptToRuntime<OpcUaServerAdapter>())
            .Collection.AddLogging(e => e.AddSimpleConsole());

        services.AddSingleton(services);
        services.AddScoped<IServiceProvider>(p => p);
        return services;
    }

    public static implicit operator List<ServiceDescriptor>(
        ServiceCollectionDataSourceAttribute servicesServerFixtureAttribute) =>
        servicesServerFixtureAttribute.Services.ToList();
}