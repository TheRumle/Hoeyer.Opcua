using Hoeyer.OpcUa.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestConfiguration;

public static class ServiceCollectionExtensions
{
    public static OnGoingOpcEntityServiceRegistration AddTestAddOpcUaServerConfiguration(
        this IServiceCollection collection, int port)
    {
        return collection.AddOpcUaServerConfiguration(conf => conf
            .WithServerId("TestServer")
            .WithServerName("Test Server")
            .WithHttpsHost("localhost", port)
            .WithEndpoints([$"opc.tcp://localhost:{port}"])
            .Build());
    }
}