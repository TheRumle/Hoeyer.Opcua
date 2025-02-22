namespace Hoeyer.OpcUa.Configuration.EntityServerBuilder;

public interface IHostStep
{
    IEndpointsStep WithHttpsHost(string host, int port);
    IEndpointsStep WithOpcTcpHost(string host, int port);
}