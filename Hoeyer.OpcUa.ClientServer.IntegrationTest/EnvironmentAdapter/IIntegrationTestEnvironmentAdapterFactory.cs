namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public interface IIntegrationTestEnvironmentAdapterFactory
{
    public IIntegrationTestEnvironmentAdapter GetIntegrationEnvironmentAdapter(string adapterId);
}