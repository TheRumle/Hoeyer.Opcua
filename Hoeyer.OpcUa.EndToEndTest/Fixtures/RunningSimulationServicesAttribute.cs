using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public sealed class RunningSimulationServicesAttribute : ServiceCollectionDataSourceAttribute
{
    private readonly ReservedPort _reservedPort;

    public RunningSimulationServicesAttribute(WebProtocol protocol = WebProtocol.OpcTcp)
    {
        _reservedPort = new ReservedPort();
        Services = CreateServiceCollection(protocol, _reservedPort.Port);
    }

    public static implicit operator List<ServiceDescriptor>(
        RunningSimulationServicesAttribute servicesAttributeServerFixture) =>
        servicesAttributeServerFixture.Services.ToList();
}