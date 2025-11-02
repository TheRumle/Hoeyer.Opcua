using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class ServerStartedHealthCheck : IServerStartedHealthCheckMarker
{
    private readonly Task _task = new(() => { });
    public bool IsServerStarted => _task.IsCompleted;

    public TaskAwaiter GetAwaiter()
    {
        return _task.GetAwaiter();
    }

    public Task ServerRunning()
    {
        return _task;
    }

    public void MarkCompleted() => _task.Start();
}