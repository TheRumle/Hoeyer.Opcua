namespace Hoeyer.OpcUa.Server.Configuration;

public record OpcUaServerOptions
{
    public int Port { get; set; }
    public string ServerName { get; set; }
}