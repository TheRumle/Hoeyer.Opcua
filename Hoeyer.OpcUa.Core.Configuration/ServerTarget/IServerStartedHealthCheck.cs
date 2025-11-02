using System.Runtime.CompilerServices;

namespace Hoeyer.OpcUa.Core.Configuration.ServerTarget;

public interface IServerStartedHealthCheck
{
    bool IsServerStarted { get; }
    TaskAwaiter GetAwaiter();
    Task ServerRunning();
}