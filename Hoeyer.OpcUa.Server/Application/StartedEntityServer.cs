using System;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class StartedEntityServer(StartableEntityServer server) : IDisposable
{
    private bool _isStopped = false;
    public void Stop()
    {
        server.Stop();
        _isStopped = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_isStopped)
        { 
            Stop();
        }
        server.Dispose();
    }
}