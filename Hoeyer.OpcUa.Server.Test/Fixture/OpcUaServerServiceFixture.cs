using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Server.IntegrationTest.Fixture;

public sealed class OpcUaServerServiceFixture
{
    public OpcUaServerServiceFixture()
    {
        ReservedPort reservedPort = new();
        var services = new ServiceCollection();
        OnGoingOpcAgentServiceRegistration = services.AddLogging(c => c.SetMinimumLevel(LogLevel.Warning))
            .AddOpcUaServerConfiguration(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithHttpsHost("localhost", reservedPort.Port)
                .WithEndpoints([$"opc.tcp://localhost:{reservedPort.Port}"])
                .Build())
            .WithAgentServices()
            .WithOpcUaServer();
    }

    public OnGoingOpcAgentServiceRegistration OnGoingOpcAgentServiceRegistration { get; set; }
}