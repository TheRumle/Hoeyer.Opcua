using System.Net;
using System.Net.Sockets;
using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

public sealed class ClientAndServerIntegrationServices : IDisposable
{
    private const string SERVER_ID = "MyServer";
    private const string SERVER_NAME = "My Server";
    private const string HOST_NAME = "localhost";

    public readonly ClientServicesAdapterArgs BaseConfiguration;
    private readonly TcpListener _portListener;

    public ClientAndServerIntegrationServices()
    {
        _portListener = new TcpListener(IPAddress.Loopback, 0);

        _portListener.Start();
        var port = ((IPEndPoint)_portListener.LocalEndpoint).Port;
        BaseConfiguration = new ClientServicesAdapterArgs
        {
            HostName = HOST_NAME,
            Port = port,
            OpcUaServerId = SERVER_ID,
            OpcUaServerName = SERVER_NAME,
            Protocol = WebProtocol.OpcTcp,
        };

        ServiceCollection = new ClientIntegrationServices(new ServiceCollection(),
                BaseConfiguration,
                [typeof(TestAssemblyMarker)],
                [typeof(TestAssemblyMarker)]
            ).OpcServices
            .WithOpcUaServer([typeof(TestAssemblyMarker).Assembly])
            .Collection;

        Scope = ServiceCollection.BuildServiceProvider().CreateScope();
        ServiceProvider = Scope.ServiceProvider;
    }


    public IServiceCollection ServiceCollection { get; private set; }

    public IServiceScope Scope { get; private set; }

    public IServiceProvider ServiceProvider { get; private set; }

    public void Dispose()
    {
        _portListener.Dispose();
    }
}