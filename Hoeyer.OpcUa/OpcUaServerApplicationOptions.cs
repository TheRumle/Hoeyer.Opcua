namespace Hoeyer.OpcUa;

public record OpcUaServerApplicationOptions
{
    public string ApplicationName { get; set; }
    public string ApplicationUri { get; set; }
}