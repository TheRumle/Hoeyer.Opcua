using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hoeyer.OpcUa.Test.Simulation;

internal class SimulationServicesTestCollection : IAsyncDisposable
{
    public SimulationServicesTestCollection(
        IServiceCollection services,
        ClientServicesAdapterArgs args,
        Type[] entityAssemblyMarkers,
        Type[] clientModelMarker
    ) : this(services, args, entityAssemblyMarkers.ToHashSet(), clientModelMarker.ToHashSet())
    {
    }

    public SimulationServicesTestCollection(
        IServiceCollection services,
        ClientServicesAdapterArgs args,
        ISet<Type> entityAssemblyMarkers,
        ISet<Type> clientModelMarker
    )
    {
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
        Scope = Collection.BuildServiceProvider().CreateAsyncScope();
    }

    public IServiceCollection Collection { get; }

    private AsyncServiceScope Scope { get; set; }
    public IServiceProvider ServiceProvider => Scope.ServiceProvider;

    public async ValueTask DisposeAsync()
    {
        await Scope.DisposeAsync();
    }
}