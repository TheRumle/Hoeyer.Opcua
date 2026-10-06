using Hoeyer.OpcUa.Fixtures.Common;
using Hoeyer.OpcUa.Fixtures.Common.Utils;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public interface IIntegrationTestEnvironment : IAsyncInitializer, IAsyncDisposable
{
    public TestDependencyProvider AvailableServices { get; }
    public OpcEnvironment OpcEnvironment { get; }
    public Task<bool> EnvironmentReady();
}