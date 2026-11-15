using Hoeyer.OpcUa.Core.Configuration.Application;
using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.Configuration;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Configuration;

internal sealed class ServerApplicationConfigurationFactory(
    IEnumerable<(string PolicyUri, MessageSecurityMode Mode)> supportedPolicies,
    IEnumerable<UserTokenPolicy> supportedTokens,
    IApplicationConfigurationTemplateFactory templateFactory,
    IOpcUaTargetServerSetup targetServerSetup,
    ServerApplicationConfigurationAction? additionalConfigure
) : IServerApplicationConfigurationFactory
{
    private readonly ServerSecurityPolicy[] _supportedSecurityPolicies =
        supportedPolicies.Select(e => new ServerSecurityPolicy()
        {
            SecurityPolicyUri = e.PolicyUri,
            SecurityMode = e.Mode
        }).ToArray();

    private readonly UserTokenPolicy[] _supportedTokenPolicies = supportedTokens.ToArray();

    public ServerApplicationConfiguration CreateServerConfiguration()
    {
        try
        {
            var template = templateFactory.CreateTemplate();
            var configuration = new ApplicationConfiguration(template.Configuration);

            configuration.ApplicationType = ApplicationType.Server;
            configuration.ServerConfiguration ??= new ServerConfiguration();

            additionalConfigure?.Invoke(new ServerApplicationConfiguration(configuration));
            ConfigureServer(configuration.ServerConfiguration);

            return new ServerApplicationConfiguration(configuration);
        }
        catch (Exception e)
        {
            throw new OpcUaEntityServerException("The created configuration was invalid", e);
        }
    }

    private void ConfigureServer(ServerConfiguration configuration)
    {
        configuration.BaseAddresses =
            targetServerSetup.Endpoints
                .Select(e => e.AbsoluteUri)
                .ToArray();

        configuration.SecurityPolicies = _supportedSecurityPolicies.ToArray();
        configuration.UserTokenPolicies = _supportedTokenPolicies.ToArray();
    }
}