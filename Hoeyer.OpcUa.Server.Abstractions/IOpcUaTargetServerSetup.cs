namespace Hoeyer.OpcUa.Server.Abstractions;

public interface IOpcUaTargetServerSetup
{
    public ISet<Uri> Endpoints { get; }
    public string ApplicationName { get; }
    public Uri ApplicationNamespace { get; }
}