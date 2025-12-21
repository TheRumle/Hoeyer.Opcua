using Hoeyer.OpcUa.EndToEndTest.Generators;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;

public sealed class ServerDependentServices(string hostName, int port) : IAsyncDisposable, IAsyncInitializer
{
    public readonly ServiceCollection Services = new ClientServiceCollection(hostName, port).Collection;

    private AsyncServiceScope Scope { get; set; }
    public IServiceProvider ServiceProvider => Scope.ServiceProvider;

    public async ValueTask DisposeAsync()
    {
        await Scope.DisposeAsync();
    }

    public Task InitializeAsync()
    {
        Scope = Services.BuildServiceProvider().CreateAsyncScope();
        return Task.CompletedTask;
    }
}