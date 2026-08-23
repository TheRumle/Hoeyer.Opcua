using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.Application;

internal sealed class ApplicationConfigurationTemplateFactory(
    IApplicationSecurityConfigurationFactory securityConfigurationFactory,
    IApplicationConfigurationRequirements targetServerInfo,
    ApplicationConfigurationSetup setup)
    : IApplicationConfigurationTemplateFactory
{
    public ApplicationConfigurationTemplate CreateTemplate()
    {
        var configuration = new ApplicationConfiguration
        {
            ApplicationName = targetServerInfo.ApplicationName,
            ApplicationUri = targetServerInfo.ApplicationNamespace.ToString(),
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier(),
                AddAppCertToTrustedStore = true
            },

            TransportConfigurations = new TransportConfigurationCollection(),

            TransportQuotas = new TransportQuotas
            {
                OperationTimeout = 5000,
                MaxStringLength = 500
            },

            ClientConfiguration = new ClientConfiguration
            {
                DefaultSessionTimeout = 60000
            },

            TraceConfiguration = new TraceConfiguration()
        };
        securityConfigurationFactory.Configure(configuration);
        var template = new ApplicationConfigurationTemplate(configuration);
        setup.Invoke(template.Configuration);

        var _ = new Uri(template.Configuration.ApplicationUri);
        return template;
    }
}