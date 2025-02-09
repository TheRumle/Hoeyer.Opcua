namespace Hoeyer.OpcUa.Server.ServiceConfiguration.Builder;

public interface IHostStep
{
    IEndpointsStep WithHost(string host);
}