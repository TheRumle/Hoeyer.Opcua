using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class ServiceCollectionDataSourceAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider ServiceProvider = CreateSharedServiceProvider();

    public IServiceCollection Services { get; protected init; } = CreateServiceCollection();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
        => ServiceProvider.CreateAsyncScope();

    public override object Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type)!;

    private static IServiceProvider CreateSharedServiceProvider() => CreateServiceCollection().BuildServiceProvider();

    protected static IServiceCollection CreateServiceCollection(WebProtocol protocol = WebProtocol.OpcTcp, int port = 5)
    {
        var services = new ServiceCollection().AddTestServices(protocol, port);
        return services;
    }

    public static implicit operator List<ServiceDescriptor>(
        ServiceCollectionDataSourceAttribute servicesServerFixtureAttribute) =>
        servicesServerFixtureAttribute.Services.ToList();
}