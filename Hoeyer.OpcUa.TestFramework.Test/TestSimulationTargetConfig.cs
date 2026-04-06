using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Test.Adapter;

namespace Hoeyer.OpcUa.TestFramework.Test;

public class TestSimulationTargetConfig : IOpcUaSimulationTarget
{
    public int NumberInits;
    public int NumberOfDispose;

    public Task InitializeAsync()
    {
        Interlocked.Increment(ref NumberInits);
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Interlocked.Increment(ref NumberOfDispose);
        return ValueTask.CompletedTask;
    }

    public int SimulationPort => 10;
    public string Host => "localhost";
    public WebProtocol Protocol => WebProtocol.Https;
    public string ServerId => "MyServer";
    public string ServerName => "MyServer";
    public string OpcApplicationName => "MyName";
    public Task<bool> HealthCheck() => Task.FromResult(true);
}