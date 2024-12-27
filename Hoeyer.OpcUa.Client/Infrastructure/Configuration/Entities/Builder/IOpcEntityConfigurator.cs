namespace Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities.Builder;


public interface IOpcEntityConfigurator<T> where T : new()
{
    public void Configure(IOpcUaEntityConfigurationBuilder<T> gantryConfiguration);
}