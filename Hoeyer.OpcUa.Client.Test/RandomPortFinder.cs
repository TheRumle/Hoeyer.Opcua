using System.Net;
using System.Net.Sockets;

namespace Hoeyer.OpcUa.ClientTest;

public static class RandomPortFinder
{
    public static int GetRandomAvailablePort()
    {
        var random = new Random();
        var port = random.Next(1024, 65535); // Ports below 1024 are typically reserved
        while (IsPortInUse(port)) port = random.Next(1024, 65535); // Try another port if it's in use

        return port;
    }

    private static bool IsPortInUse(int port)
    {
        try
        {
            using var tcpListener = new TcpListener(IPAddress.Loopback, port);
            tcpListener.Start();
            tcpListener.Stop();
            return false;
        }
        catch (SocketException)
        {
            return true;
        }
    }
}