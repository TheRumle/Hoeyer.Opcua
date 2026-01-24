using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Test.Simulation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hoeyer.OpcUa.Test;

internal class ServerDependentClientSetup
{
    public ServerDependentClientSetup(ClientServicesAdapterArgs args,
        ISet<Type> entityAssemblyMarkers,
        ISet<Type> clientModelMarker
    )
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddSingleton(services)
            .AddScoped<IServiceProvider>(p => p)
            .AddOpcUa(conf => conf
                .WithServerId(args.OpcUaServerId)
                .WithServerName(args.OpcUaServerName)
                .WithWebOrigins(
                    args.Protocol,
                    args.HostName,
                    args.Port
                )
                .Build())
            .WithEntityModelsFrom(entityAssemblyMarkers)
            .WithOpcUaClientModelsFrom(clientModelMarker);

        Collection = services;
    }

    public IServiceCollection Collection { get; }

    private AsyncServiceScope Scope { get; set; }
    public IServiceProvider ServiceProvider => Scope.ServiceProvider;

    public async ValueTask DisposeAsync()
    {
        await Scope.DisposeAsync();
    }

    public Task InitializeAsync()
    {
        Scope = Collection.BuildServiceProvider().CreateAsyncScope();
        return Task.CompletedTask;
    }
}