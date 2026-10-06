using Hoeyer.OpcUa.Fixtures.Common;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.DataSources;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public interface IIntegrationTestEnvironment : IAsyncInitializer, IAsyncDisposable
{
    public IntegrationTestServiceProvider AvailableServices { get; }
    public OpcEnvironment OpcEnvironment { get; }
    public Task<bool> EnvironmentReady();
}