namespace Hoeyer.OpcUa.Server.Configuration;

public record OpcUaServerApplicationOptions
{
    public string ApplicationName { get; set; }
    public string ApplicationUri { get; set; }
}