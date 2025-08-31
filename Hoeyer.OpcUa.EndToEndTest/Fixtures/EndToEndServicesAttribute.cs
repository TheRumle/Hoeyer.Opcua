using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.TestEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public sealed class EndToEndServicesAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    public EndToEndServicesAttribute()
    {
        ReservedPort reservedPort = new();
        ServiceCollection = new ServiceCollection()
            .AddTestEntityServices(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithHttpsHost("localhost", reservedPort.Port)
                .WithEndpoints([$"opc.tcp://localhost:{reservedPort.Port}"])
                .Build());
    }

    public IServiceCollection ServiceCollection { get; }

    public static implicit operator List<ServiceDescriptor>(
        EndToEndServicesAttribute servicesServerFixtureAttribute) =>
        servicesServerFixtureAttribute.ServiceCollection.ToList();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        ServiceCollection.BuildServiceProvider().CreateScope();

    public override object? Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type);
}