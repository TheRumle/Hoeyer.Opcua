using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public interface IIntegrationTestEnvironment : IAsyncInitializer, IAsyncDisposable
{
    public IServiceCollection AvailableServices { get; }

    public OpcEnvironment OpcEnvironment { get; }
    public Task<bool> EnvironmentReady();
}