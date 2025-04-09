using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Core;

public sealed class EntityServerStartedMarker
{
    private readonly Task task = new(() => { });

    internal void MarkCompleted() => task.Start();

    public TaskAwaiter GetAwaiter()
    {
        return task.GetAwaiter();
    }
    
    public Task ServerRunning()
    {
        return task;
    }
}