namespace Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;

public interface IApplicationTargetConfigurationBuildable
{
    IApplicationConfigurationRequirements Build();
}