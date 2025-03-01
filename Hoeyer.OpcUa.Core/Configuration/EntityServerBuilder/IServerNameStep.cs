namespace Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;

public interface IServerNameStep
{
    IHostStep WithServerName(string serverName);
}