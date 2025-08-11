using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Api;

public sealed class AgentServerStartedMarker
{
    private readonly Task _task = new(() => { });
    public bool IsServerStarted => _task.IsCompleted;

    internal void MarkCompleted() => _task.Start();

    public TaskAwaiter GetAwaiter()
    {
        return _task.GetAwaiter();
    }

    public Task ServerRunning()
    {
        return _task;
    }
}