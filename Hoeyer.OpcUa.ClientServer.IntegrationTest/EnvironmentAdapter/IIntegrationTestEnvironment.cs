using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public interface IIntegrationTestEnvironment : IAsyncInitializer, IAsyncDisposable
{
    public IServiceProvider AvailableServices { get; }
    public OpcEnvironment OpcEnvironment { get; }
    public Task<bool> EnvironmentReady();
}