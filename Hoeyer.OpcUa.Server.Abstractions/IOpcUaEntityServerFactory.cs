namespace Hoeyer.OpcUa.Server.Abstractions;

public interface IOpcUaEntityServerFactory
{
    IStartableEntityServer CreateServer();
}