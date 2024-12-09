namespace Hoeyer.Machines.OpcUa.Configuration.Entity;


public interface IOpcUaNodeConfiguration<T>
{
    public void Configure(IOpcUaEntityConfigurationBuilder<T> gantryConfiguration);
}