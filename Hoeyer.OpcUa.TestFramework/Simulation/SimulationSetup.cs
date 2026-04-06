using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Test.Adapter;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Test.Simulation;

public class SimulationSetup(ITestFrameworkAdapter adapter) : IAsyncDisposable, IAsyncInitializer
{
    private readonly SemaphoreSlim semaphore = new(1);
    public readonly Guid SimulationTestIdentity = Guid.CreateVersion7();
    private SimulationServicesTestCollection _clientSetup = null!;

    private bool _isInitialized;
    public ITestFrameworkAdapter Adapter => adapter;
    public IOpcUaSimulationTarget SimulationTarget { get; private set; } = null!;
    public IEntitySessionFactory SessionFactory { get; private set; } = null!;
    public IServiceCollection ClientServices { get; private set; } = null!;
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    public EntityTypesCollection EntityCollection => ServiceProvider.GetRequiredService<EntityTypesCollection>();


    public async ValueTask DisposeAsync()
    {
        await SimulationTarget.DisposeAsync();
        await _clientSetup.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await using (await semaphore.LockAsync())
        {
            if (_isInitialized)
            {
                return;
            }

            SimulationTarget = adapter.SimulationTarget;
            await SimulationTarget.InitializeAsync();

            var clientAdaptionArguments = new ClientServicesAdapterArgs
            {
                HostName = SimulationTarget.Host,
                Port = SimulationTarget.SimulationPort,
                OpcUaServerId = SimulationTarget.ServerId,
                OpcUaServerName = SimulationTarget.ServerName,
                Protocol = SimulationTarget.Protocol
            };

            _clientSetup = new SimulationServicesTestCollection(
                adapter.ApplicationServices,
                clientAdaptionArguments,
                adapter.EntityAssemblyMarkers,
                adapter.ClientAssemblyMarkers
            );

            ClientServices = _clientSetup.Collection;
            ServiceProvider = _clientSetup.ServiceProvider;
            SessionFactory = ServiceProvider.GetRequiredService<IEntitySessionFactory>();
            _isInitialized = true;
        }
    }
}