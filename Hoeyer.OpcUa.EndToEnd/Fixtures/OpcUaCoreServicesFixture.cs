using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.EndToEndTest.TestApplication;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public class OpcUaCoreServicesFixture
{
    protected OnGoingOpcEntityServiceRegistration OnGoingOpcEntityServiceRegistration { get; private set; }
    public IServiceCollection ServiceCollection => OnGoingOpcEntityServiceRegistration.Collection;
    public OpcUaCoreServicesFixture()
    {
        ReservedPort reservedPort = new();
        OnGoingOpcEntityServiceRegistration = new ServiceCollection().AddOpcUaServerConfiguration(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithHttpsHost("localhost", reservedPort.Port)
                .WithEndpoints([$"opc.tcp://localhost:{reservedPort.Port}"])
                .Build())
            .WithEntityServices();
    }
}