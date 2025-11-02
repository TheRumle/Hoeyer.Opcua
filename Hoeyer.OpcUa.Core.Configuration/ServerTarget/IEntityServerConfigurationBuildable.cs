namespace Hoeyer.OpcUa.Core.Configuration.ServerTarget;

public interface IEntityServerConfigurationBuildable
{
    IOpcUaTargetServerInfo Build();
}