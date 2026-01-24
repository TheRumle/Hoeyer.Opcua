using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Test.Simulation;

public interface IOpcUaSimulationServer : IAsyncInitializer, IAsyncDisposable
{
    int SimulationPort { get; }
    string Host { get; }
    WebProtocol Protocol { get; }
    string ServerId { get; }
    string ServerName { get; }
    string OpcApplicationName { get; }

    public Task<bool> HealthCheck();
}