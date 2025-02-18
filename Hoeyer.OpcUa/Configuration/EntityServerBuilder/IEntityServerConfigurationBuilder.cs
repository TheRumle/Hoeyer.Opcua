namespace Hoeyer.OpcUa.Configuration.EntityServerBuilder;

public interface IEntityServerConfigurationBuilder
{
    IServerNameStep WithServerId(string serverId);
}