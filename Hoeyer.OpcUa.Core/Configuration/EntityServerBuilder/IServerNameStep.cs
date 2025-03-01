namespace Hoeyer.OpcUa.Configuration.EntityServerBuilder;

public interface IServerNameStep
{
    IHostStep WithServerName(string serverName);
}