namespace Hoeyer.OpcUa.Server.ServiceConfiguration.Builder;

public interface IEntityServerConfigurationBuilder
{
    IServerNameStep WithServerId(string serverId);
}