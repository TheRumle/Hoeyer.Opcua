namespace Hoeyer.OpcUa.Core.Configuration.AgentServerBuilder;

public interface IHostStep
{
    IEndpointsStep WithHttpsHost(string host, int port);
    IEndpointsStep WithOpcTcpHost(string host, int port);
}