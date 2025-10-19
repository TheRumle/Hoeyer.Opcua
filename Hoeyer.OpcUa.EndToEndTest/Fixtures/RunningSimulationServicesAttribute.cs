using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public sealed class RunningSimulationServicesAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private readonly ReservedPort _reservedPort;

    public RunningSimulationServicesAttribute(WebProtocol protocol = WebProtocol.OpcTcp)
    {
        _reservedPort = new ReservedPort();
        ServiceCollection = new ServiceCollection()
            .AddOpcUa(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithWebOrigins(protocol, "localhost", _reservedPort.Port)
                .Build())
            .WithEntityServices()
            .WithOpcUaClientServices()
            .WithOpcUaSimulationServices(configure =>
            {
                configure.WithTimeScaling(double.Epsilon);
                configure.AdaptToRuntime<OpcUaServerAdapter>();
            })
            .WithOpcUaServerAsBackgroundService()
            .Collection.AddLogging(e => e.AddSimpleConsole());
    }

    public IServiceCollection ServiceCollection { get; }

    public static implicit operator List<ServiceDescriptor>(
        RunningSimulationServicesAttribute servicesAttributeServerFixture) =>
        servicesAttributeServerFixture.ServiceCollection.ToList();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        ServiceCollection.BuildServiceProvider().CreateScope();

    public override object? Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type);
}