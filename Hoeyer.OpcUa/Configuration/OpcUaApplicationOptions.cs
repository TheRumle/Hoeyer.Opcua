namespace Hoeyer.OpcUa.Configuration;

public record OpcUaApplicationOptions
{
    public string ApplicationName { get; set; } = null!;
    public string ApplicationUri { get; set; } = null!;
}