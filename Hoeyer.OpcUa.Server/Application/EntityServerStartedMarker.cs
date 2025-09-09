using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class EntityServerStartedMarker : IEntityServerStartedMarker
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

    internal void MarkCompleted() => _task.Start();
}