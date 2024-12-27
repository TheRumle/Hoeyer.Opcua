namespace Hoeyer.OpcUa.Server.Configuration;

public record OpcUaApplicationOptions
{
    public string ApplicationName { get; set; }
    public string ApplicationUri { get; set; }
}