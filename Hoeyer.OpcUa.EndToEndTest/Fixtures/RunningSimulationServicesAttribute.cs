using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.EntityModelling;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public sealed class RunningSimulationServicesAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private readonly ReservedPort _reservedPort;

    public RunningSimulationServicesAttribute()
    {
        _reservedPort = new ReservedPort();
        ServiceCollection = new ServiceCollection()
            .AddRunningTestEntityServices(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithHttpsHost("localhost", _reservedPort.Port)
                .WithEndpoints([$"opc.tcp://localhost:{_reservedPort.Port}"])
                .Build());
    }

    public IServiceCollection ServiceCollection { get; }

    public static implicit operator List<ServiceDescriptor>(
        RunningSimulationServicesAttribute servicesAttributeServerFixture) =>
        servicesAttributeServerFixture.ServiceCollection.ToList();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        ServiceCollection.BuildServiceProvider().CreateScope();

    public override object? Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type);
}