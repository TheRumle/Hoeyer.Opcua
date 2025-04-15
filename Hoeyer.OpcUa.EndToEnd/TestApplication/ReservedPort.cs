using System.Net;
using System.Net.Sockets;

namespace Hoeyer.OpcUa.EndToEndTest.TestApplication;

internal sealed record ReservedPort : IDisposable
{
    public static implicit operator int(ReservedPort port) => port.Port;
    private readonly TcpListener _listener = new(IPAddress.Loopback, 0);

    private int? _port = null;

    private int GetPort()
    {
        if (_port is not null) return _port.Value;
        _listener.Start();
        _port = ((IPEndPoint)_listener.LocalEndpoint).Port;
        _listener.Stop();
        return _port.Value;
    }
    
    public int Port => GetPort();

    /// <inheritdoc />
    public void Dispose()
    {
        _listener.Dispose();
    }
}