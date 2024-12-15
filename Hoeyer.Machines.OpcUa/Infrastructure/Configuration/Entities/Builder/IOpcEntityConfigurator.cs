namespace Hoeyer.Machines.OpcUa.Entities.Configuration.Builder;


public interface IOpcEntityConfigurator<T> where T : new()
{
    public void Configure(IOpcUaEntityConfigurationBuilder<T> gantryConfiguration);
}