using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Test.Adapter;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Test.Simulation;

public class SimulationSetup(
    IOpcUaSimulationServer simulationServer,
    ISet<Type> entityAssemblyMarkers,
    ISet<Type> clientAssemblyMarkers) : IAsyncDisposable, IAsyncInitializer
{
    private SimulationServicesCollection _clientSetup = null!;
    public IEntitySessionFactory SessionFactory { get; private set; }
    public IServiceCollection ClientServices { get; private set; }
    public IServiceProvider ServiceProvider { get; private set; }
    public EntityTypesCollection EntityCollection => ServiceProvider.GetRequiredService<EntityTypesCollection>();


    public async ValueTask DisposeAsync()
    {
        await simulationServer.DisposeAsync();
        await _clientSetup.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await simulationServer.InitializeAsync();
        var clientAdaptionArguments = new ClientServicesAdapterArgs
        {
            HostName = simulationServer.Host,
            Port = simulationServer.SimulationPort,
            OpcUaServerId = simulationServer.ServerId,
            OpcUaServerName = simulationServer.ServerName,
            Protocol = simulationServer.Protocol
        };
        _clientSetup = new SimulationServicesCollection
        (
            clientAdaptionArguments,
            entityAssemblyMarkers,
            clientAssemblyMarkers
        );

        ClientServices = _clientSetup.Collection;
        ServiceProvider = _clientSetup.ServiceProvider;
        SessionFactory = ServiceProvider.GetRequiredService<IEntitySessionFactory>();
    }
}