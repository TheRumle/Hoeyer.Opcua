using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Playground.Application.EndToEndTest.Environment.Adapter.TestContainer;

public sealed class PlaygroundTestContainer : IIntegrationTestEnvironment
{
    public const string OPCUA_SERVERID = "HostedSimulation";
    public const string OPCUA_SERVERNAME = "HostedSimulation";
    public const string OPCUA_APPLICATION_NAME = "Simulation";
    private readonly Lazy<Task> _initializeTask;
    private readonly string _containerName;
    private readonly IServiceCollection _services = new ServiceCollection();

    public PlaygroundTestContainer(WebProtocol webProtocol, string containerName)
    {
        Protocol = webProtocol;
        _containerName = containerName;
        _initializeTask = new Lazy<Task>(StartAndWaitForHealth);
    }

    public DockerHealthChecker HealthChecker { get; set; } = null!;

    public IContainer Container { get; private set; } = null!;
    public WebProtocol Protocol { get; }

    public int SimulationPort { get; private set; }
    public string Host => Container.Hostname;
    public string ServerId => OPCUA_SERVERID;
    public string ServerName => OPCUA_SERVERNAME;
    public IServiceProvider Services { get; set; }
    public OpcEnvironment OpcEnvironment { get; private set; } = null!;
    public async Task<bool> EnvironmentReady() => await HealthChecker.IsHealthy();
    public IServiceProvider AvailableServices { get; private set; }


    public async ValueTask DisposeAsync()
    {
        Console.WriteLine($"Disposing {nameof(PlaygroundTestContainer)}");
        await Container.DisposeAsync();
    }

    public Task InitializeAsync()
    {
        if (_initializeTask.IsValueCreated)
        {
            return Task.CompletedTask;
        }

        return _initializeTask.Value;
    }

    private async Task StartAndWaitForHealth()
    {
        Container = new ContainerBuilder("playground/simulationserver:latest")
            .WithName(_containerName)
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

        OpcEnvironment = new OpcEnvironment
        {
            HostName = Host,
            Port = SimulationPort,
            OpcUaServerId = ServerId,
            OpcUaServerName = ServerName,
            Protocol = Protocol
        };
        _services.AddClientTestServices(OpcEnvironment, [typeof(TestEntity)]);
        AvailableServices = _services.BuildServiceProvider();

        HealthChecker = new DockerHealthChecker(Container);

        Console.WriteLine("Attempting to start {0}...", _containerName);
        await Container.StartAsync();

        await EnvironmentReady();
        SimulationPort = Container.GetMappedPublicPort(4840);
        Console.WriteLine($"{nameof(PlaygroundTestContainer)} initialized");
    }
}