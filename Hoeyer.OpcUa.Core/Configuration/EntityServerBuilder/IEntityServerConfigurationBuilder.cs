namespace Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;

public interface IEntityServerConfigurationBuilder
{
    IServerNameStep WithServerId(string serverId);
}