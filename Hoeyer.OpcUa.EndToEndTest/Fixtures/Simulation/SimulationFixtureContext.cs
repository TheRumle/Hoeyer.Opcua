using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;

public sealed class SimulationFixtureContext : IAsyncInitializer, IAsyncDisposable
{
    public IServiceProvider ServiceProvider => ServerDependentServices.ServiceProvider;
    public ServerDependentServices ServerDependentServices { get; private set; } = null!;
    private OpcUaSimulationServerContainer OpcUaSimulationServerContainer { get; set; } = null!;

    public async ValueTask DisposeAsync()
    {
        await ServerDependentServices.DisposeAsync();
        await OpcUaSimulationServerContainer.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        OpcUaSimulationServerContainer = new OpcUaSimulationServerContainer();
        await OpcUaSimulationServerContainer.InitializeAsync();
        ServerDependentServices =
            new ServerDependentServices(OpcUaSimulationServerContainer.Host, OpcUaSimulationServerContainer.Port);
        await ServerDependentServices.InitializeAsync();
    }
}