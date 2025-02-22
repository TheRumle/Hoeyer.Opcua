using System;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class StartedEntityServer(StartableEntityServer server) : IDisposable
{
    private bool _isStopped;

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_isStopped) Stop();
        server.Dispose();
    }

    public void Stop()
    {
        server.Dispose();
        _isStopped = true;
    }
}