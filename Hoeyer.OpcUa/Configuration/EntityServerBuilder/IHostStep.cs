namespace Hoeyer.OpcUa.Configuration.EntityServerBuilder;

public interface IHostStep
{
    IEndpointsStep WithHttpHost(string host);
    IEndpointsStep WithOpcTcpHost(string host);
}