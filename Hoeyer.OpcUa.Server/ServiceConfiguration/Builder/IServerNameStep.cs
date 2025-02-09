namespace Hoeyer.OpcUa.Server.ServiceConfiguration.Builder;

public interface IServerNameStep
{
    IHostStep WithServerName(string serverName);
}