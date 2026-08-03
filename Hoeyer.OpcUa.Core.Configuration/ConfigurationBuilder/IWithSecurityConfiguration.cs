namespace Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;

public interface IWithSecurityConfiguration
{
    IApplicationTargetConfigurationBuildable WithSecurityConfiguration(CertificateConfiguration options);
}