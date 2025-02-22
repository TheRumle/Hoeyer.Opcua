namespace Hoeyer.OpcUa.Configuration.EntityServerBuilder;

public interface IEntityServerConfigurationBuildable
{
    IOpcUaEntityServerInfo Build();
}