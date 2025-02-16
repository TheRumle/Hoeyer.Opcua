namespace Hoeyer.OpcUa.Server.ServiceConfiguration.Builder;

public interface IHostStep
{
    IEndpointsStep WithHttpHost(string host);
    IEndpointsStep WithOpcTcpHost(string host);
}