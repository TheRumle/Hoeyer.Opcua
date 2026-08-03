namespace Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;

public interface IWithOriginsStep
{
    public IWithApplicationUri WithWebOrigins(WebProtocol protocol, string host, int port);
}