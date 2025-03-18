using System.Net;
using System.Net.Sockets;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Hoeyer.OpcUa.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using TestConfiguration;

namespace Hoeyer.OpcUa.ClientTest;

public static class RandomPortFinder
{
    public static int GetRandomAvailablePort()
    {
        Random random = new Random();
        int port = random.Next(1024, 65535); // Ports below 1024 are typically reserved
        while (IsPortInUse(port))
        {
            port = random.Next(1024, 65535); // Try another port if it's in use
        }

        return port;
    }

    private static bool IsPortInUse(int port)
    {
        try
        {
            using var tcpListener = new TcpListener(IPAddress.Loopback, port);
            tcpListener.Start();
            tcpListener.Stop();
            return false; 
        }
        catch (SocketException)
        {
            return true;
        }
    }
}




public sealed class DependencyInjectionAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    
    
    private static readonly int port = 5000;

    private static Func<Task<IFutureDockerImage>> CreateImage = async () =>
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
        var dockerfilePath = Path.Combine(projectRoot, "Hoeyer.OpcUa.Server.Hosted");
        var image = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(dockerfilePath)
            .WithDockerfile("Dockerfile") // Relative path to the Dockerfile
            .WithDeleteIfExists(false)
            .Build();

        await image.CreateAsync();
        return image;
    };

    private static readonly Func<Task<IContainer>> CreateContainer = async () =>
    {
        var container = new ContainerBuilder()
            .WithExposedPort(port)
            .WithPortBinding(port, port)
            .WithImage(Image!.FullName)
            .WithCleanUp(true).Build();

        await container.StartAsync();
        return container;
    };

    private static readonly IFutureDockerImage Image = CreateImage.Invoke().Result;
    private static readonly IContainer OpcUaServerContainer = CreateContainer.Invoke().Result;

    private static IServiceProvider ServiceProvider = CreateServiceProvider();

    
    private static ServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .AddSingleton<OpcUaServerContainer>(_ => new OpcUaServerContainer(OpcUaServerContainer, port))
            .AddTestAddOpcUaServerConfiguration(port)
            .AddOpcUaClientServices()
            .Collection
            .BuildServiceProvider();
    }



    /// <inheritdoc />
    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        ServiceProvider.CreateAsyncScope();

    /// <inheritdoc />
    public override object? Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type);
}

public sealed record OpcUaServerContainer(IContainer ServerContainer, int PortToServer) : IAsyncDisposable
{
    /// <inheritdoc />
    public async ValueTask DisposeAsync() => await ServerContainer.DisposeAsync();
}

