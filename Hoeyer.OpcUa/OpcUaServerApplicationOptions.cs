namespace Hoeyer.OpcUa;

public record OpcUaServerApplicationOptions
{
    public string ApplicationName { get; set; } = null!;
    public string ApplicationUri { get; set; } = null!;
}