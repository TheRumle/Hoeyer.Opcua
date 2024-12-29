namespace Hoeyer.OpcUa.Client.Configuration.Entities.Builder;


public interface IOpcEntityConfigurator<T> where T : new()
{
    public void Configure(IOpcUaEntityConfigurationBuilder<T> gantryConfiguration);
}