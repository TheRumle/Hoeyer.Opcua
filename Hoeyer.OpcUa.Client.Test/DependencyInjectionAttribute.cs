using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Hoeyer.OpcUa.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestConfiguration;

namespace Hoeyer.OpcUa.ClientTest;

public sealed class DependencyInjectionAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly int port = 5000;

    private static readonly Func<Task<IFutureDockerImage>> CreateImage = async () =>
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

    private static readonly IServiceProvider ServiceProvider = CreateServiceProvider();


    private static ServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .AddSingleton<OpcUaServerContainer>(_ => new OpcUaServerContainer(OpcUaServerContainer, port))
            .AddTestAddOpcUaServerConfiguration(port)
            .WithEntityServices()
            .Collection
            .BuildServiceProvider();
    }


    /// <inheritdoc />
    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return ServiceProvider.CreateAsyncScope();
    }

    /// <inheritdoc />
    public override object? Create(IServiceScope scope, Type type)
    {
        return scope.ServiceProvider.GetService(type);
    }
}