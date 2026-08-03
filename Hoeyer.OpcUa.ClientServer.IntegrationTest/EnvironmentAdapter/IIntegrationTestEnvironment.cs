using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public interface IIntegrationTestEnvironment : IAsyncInitializer, IAsyncDisposable
{
    int SimulationPort { get; }
    string Host { get; }
    WebProtocol Protocol { get; }
    string ServerId { get; }
    string ServerName { get; }
    public Task<bool> EnvironmentReady();
}