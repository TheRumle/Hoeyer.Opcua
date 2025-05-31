using System.Net;
using System.Net.Sockets;

namespace Hoeyer.OpcUa.Core.Test.Fixtures;

internal sealed record ReservedPort : IDisposable
{
    private readonly TcpListener _listener = new(IPAddress.Loopback, 0);

    private int? _port;

    public int Port => GetPort();

    /// <inheritdoc />
    public void Dispose()
    {
        _listener.Dispose();
    }

    public static implicit operator int(ReservedPort port) => port.Port;

    private int GetPort()
    {
        if (_port is not null) return _port.Value;
        _listener.Start();
        _port = ((IPEndPoint)_listener.LocalEndpoint).Port;
        _listener.Stop();
        return _port.Value;
    }
}