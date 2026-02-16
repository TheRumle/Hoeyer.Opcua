using System.Net;
using System.Net.Sockets;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Test.Adapter;

namespace Playground.Application.EndToEndTest;

public sealed class OpcUaServerTestContainer(WebProtocol webProtocol)
    : IOpcUaSimulationServer
{
    public const string OPCUA_SERVERID = "HostedSimulation";
    public const string OPCUA_SERVERNAME = "HostedSimulation";
    public const string OPCUA_APPLICATION_NAME = "Simulation";
    private TcpListener _portHolder = null!;

    public OpcUaServerTestContainer() : this(WebProtocol.OpcTcp)
    {
    }

    public IContainer Container { get; private set; } = null!;
    public INetwork Network { get; set; } = null!;

    public WebProtocol Protocol => webProtocol;
    public int SimulationPort { get; private set; }
    public string Host => Container.Hostname;
    public string ServerId => OPCUA_SERVERID;
    public string ServerName => OPCUA_SERVERNAME;
    public string OpcApplicationName => OPCUA_APPLICATION_NAME;
    public Task<bool> HealthCheck() => Task.FromResult(Container.Health == TestcontainersHealthStatus.Healthy);

    public async ValueTask DisposeAsync()
    {
        await Container.DisposeAsync();
        _portHolder.Dispose();
    }

    public async Task InitializeAsync()
    {
        var containerPrefix = Guid.NewGuid().ToString();

        Container = new ContainerBuilder()
            .WithImage("playground/simulationserver:latest")
            .WithName($"simulation-server-{containerPrefix}")
            .WithPortBinding(4840, true)
            .WithEnvironment("OPCUA_PORT", "4840")
            .WithEnvironment("OPCUA_PROTOCOL", Protocol.ToString())
            .WithEnvironment("OPCUA_SERVERID", OPCUA_SERVERID)
            .WithEnvironment("OPCUA_SERVERNAME", OPCUA_SERVERNAME)
            .WithEnvironment("OPCUA_APPLICATION_NAME", OPCUA_APPLICATION_NAME)
            .WithEnvironment("LogLevel", "Information")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
            .WithImagePullPolicy(PullPolicy.Missing)
            .WithCleanUp(true)
            .Build();

        await Container.StartAsync();
        SimulationPort = Container.GetMappedPublicPort(4840);
        _portHolder = new TcpListener(IPAddress.Loopback, SimulationPort);
    }
}