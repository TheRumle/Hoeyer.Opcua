namespace Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;

public interface IWithOriginsStep
{
    public IEntityServerConfigurationBuildable WithWebOrigins(WebProtocol protocol, string host, int port);
}