using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.EndToEndTest.TestApplication;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class OpcUaCoreServicesFixture
{
    private ReservedPort _reservedPort = new();
    protected OnGoingOpcEntityServiceRegistration OnGoingOpcEntityServiceRegistration { get; private set; }
    public IServiceCollection ServiceCollection => OnGoingOpcEntityServiceRegistration.Collection;
    public OpcUaCoreServicesFixture()
    {
        ServiceCollection.AddOpcUaServerConfiguration(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithHttpsHost("localhost", _reservedPort.Port)
                .WithEndpoints([$"opc.tcp://localhost:{_reservedPort.Port}"])
                .Build())
            .WithEntityServices();
    }
}