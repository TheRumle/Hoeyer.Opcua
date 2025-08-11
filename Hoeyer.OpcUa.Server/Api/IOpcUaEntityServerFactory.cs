namespace Hoeyer.OpcUa.Server.Api;

public interface IOpcUaEntityServerFactory
{
    IStartableEntityServer CreateServer();
}