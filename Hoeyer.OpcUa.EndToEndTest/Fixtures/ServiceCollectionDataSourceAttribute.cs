using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Test.Fixtures;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class ServiceCollectionDataSourceAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider ServiceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
        =>
            ServiceProvider.CreateAsyncScope();

    public override object Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type);

    private static IServiceProvider CreateSharedServiceProvider() => CreateServiceCollection().BuildServiceProvider();

    private static IServiceCollection CreateServiceCollection()
    {
        OpcUaCoreServicesFixture fixture = new();
        fixture.OnGoingOpcEntityServiceRegistration.WithOpcUaClientServices().WithOpcUaServer();
        fixture.ServiceCollection.AddSingleton(fixture.ServiceCollection);
        return fixture.ServiceCollection;
    }
}