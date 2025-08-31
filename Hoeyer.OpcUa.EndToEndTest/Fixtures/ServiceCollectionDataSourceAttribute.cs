using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Test.Fixtures;
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
        =>
            ServiceProvider.CreateAsyncScope();

    public override object Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type)!;

    private static IServiceProvider CreateSharedServiceProvider() => CreateServiceCollection().BuildServiceProvider();

    private static IServiceCollection CreateServiceCollection()
    {
        OpcUaCoreServicesFixtureAttribute fixtureAttribute = new();
        fixtureAttribute.OnGoingOpcEntityServiceRegistration
            .WithOpcUaClientServices()
            .WithOpcUaServer()
            .WithOpcUaSimulationServices(configure =>
            {
                configure.WithTimeScaling(double.Epsilon);
                configure.AdaptToRuntime<ServerSimulationAdapter>();
            })
            .Collection.AddLogging(e => e.AddSimpleConsole());
        fixtureAttribute.ServiceCollection.AddSingleton(fixtureAttribute.ServiceCollection);
        fixtureAttribute.ServiceCollection.AddScoped<IServiceProvider>(p => p);
        return fixtureAttribute.ServiceCollection;
    }

    public static implicit operator List<ServiceDescriptor>(
        ServiceCollectionDataSourceAttribute servicesServerFixtureAttribute) =>
        servicesServerFixtureAttribute.Services.ToList();
}