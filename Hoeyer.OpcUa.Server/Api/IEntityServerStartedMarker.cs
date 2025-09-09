using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Api;

public interface IEntityServerStartedMarker
{
    bool IsServerStarted { get; }
    TaskAwaiter GetAwaiter();
    Task ServerRunning();
}