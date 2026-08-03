using Hoeyer.OpcUa.Client.Abstractions.Configuration;
using Hoeyer.OpcUa.Core.Configuration.Application;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Configuration;

internal sealed class ClientApplicationConfigurationFactory(
    IApplicationConfigurationTemplateFactory template,
    ClientApplicationConfigurationAction configurationAction
)
    : IClientApplicationConfigurationFactory
{
    public ApplicationConfiguration CreateClientConfiguration()
    {
        var clientApplicationConfiguration = new ApplicationConfiguration(template.CreateTemplate().Configuration);
        clientApplicationConfiguration.ApplicationType = ApplicationType.Client;
        configurationAction.Invoke(clientApplicationConfiguration);
        return clientApplicationConfiguration;
    }
}

internal delegate void ClientApplicationConfigurationAction(ApplicationConfiguration applicationConfiguration);

public sealed record ClientApplicationConfiguration(ApplicationConfiguration ApplicationConfiguration);