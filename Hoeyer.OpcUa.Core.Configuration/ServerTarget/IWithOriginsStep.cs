namespace Hoeyer.OpcUa.Core.Configuration.ServerTarget;

public interface IWithOriginsStep
{
    public IWithApplicationUri WithWebOrigins(WebProtocol protocol, string host, int port);
}