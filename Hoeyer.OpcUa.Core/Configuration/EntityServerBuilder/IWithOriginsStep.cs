namespace Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;

public interface IWithOriginsStep
{
    public IWithApplicationUri WithWebOrigins(WebProtocol protocol, string host, int port);
}