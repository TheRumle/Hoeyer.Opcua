namespace Hoeyer.OpcUa.Server.Api;

public interface IOpcUaAgentServerFactory
{
    IStartableAgentServer CreateServer();
}