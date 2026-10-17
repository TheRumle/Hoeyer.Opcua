using Hoeyer.OpcUa.Core.Configuration.Application;
using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.Configuration;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Configuration;

internal sealed class ServerApplicationConfigurationFactory(
    IApplicationConfigurationTemplateFactory templateFactory,
    IOpcUaTargetServerSetup targetServerSetup,
    ServerApplicationConfigurationAction? additionalConfigure
)
    : IServerApplicationConfigurationFactory
{
    private static readonly ServerSecurityPolicy[] DefaultSupportedSecurityPolicies =
        new List<(string Uri, MessageSecurityMode Mode)>
        {
            ("https://opcfoundation.org/UA/SecurityPolicy#None", MessageSecurityMode.None)
            //("https://opcfoundation.org/UA/SecurityPolicy#Basic256", MessageSecurityMode.Sign),
            //("https://opcfoundation.org/UA/SecurityPolicy#Basic256", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Basic256", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256", MessageSecurityMode.Sign),
            //("https://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Basic128Rsa15", MessageSecurityMode.Sign),
            //("https://opcfoundation.org/UA/SecurityPolicy#Basic128Rsa15", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Basic128Rsa15", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Aes128_Sha256_RsaOaep", MessageSecurityMode.Sign),
            //("https://opcfoundation.org/UA/SecurityPolicy#Aes128_Sha256_RsaOaep", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Aes128_Sha256_RsaOaep", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Aes256_Sha256_RsaPss", MessageSecurityMode.Sign),
            //("https://opcfoundation.org/UA/SecurityPolicy#Aes256_Sha256_RsaPss", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Aes256_Sha256_RsaPss", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Https", MessageSecurityMode.Sign),
            //("https://opcfoundation.org/UA/SecurityPolicy#Https", MessageSecurityMode.SignAndEncrypt),
            //("https://opcfoundation.org/UA/SecurityPolicy#Https", MessageSecurityMode.SignAndEncrypt)
        }.Select(e => new ServerSecurityPolicy
        {
            SecurityPolicyUri = e.Uri,
            SecurityMode = e.Mode
        }).ToArray();

    private static readonly UserTokenPolicy[] SupportedTokenPolicies = new List<UserTokenPolicy>
    {
        new(UserTokenType.Anonymous),
        new(UserTokenType.UserName),
        new(UserTokenType.Certificate),
        new(UserTokenType.IssuedToken)
    }.ToArray();

    public ApplicationConfiguration CreateServerConfiguration()
    {
        try
        {
            var template = templateFactory.CreateTemplate();
            var templateCopy = new ApplicationConfiguration(template.Configuration);
            templateCopy.ServerConfiguration ??= new ServerConfiguration();
            templateCopy.ApplicationType = ApplicationType.Server;
            additionalConfigure?.Invoke(templateCopy);
            
            var configuration = templateCopy.ServerConfiguration;
            ConfigureServer(configuration);

            return templateCopy;
        }
        catch (Exception e)
        {
            throw new OpcUaEntityServerException("The created configuration was invalid", e);
        }
    }

    private void ConfigureServer(ServerConfiguration configuration)
    {
        configuration.BaseAddresses = targetServerSetup.Endpoints.Select(e => e.AbsoluteUri).ToArray();
        if (configuration.BaseAddresses is not { Count: > 0 })
        {
            configuration.BaseAddresses = targetServerSetup.Endpoints.Select(e => e.AbsoluteUri).ToArray();
        }

        if (configuration.SecurityPolicies is not { Count: > 0 })
        {
            configuration.SecurityPolicies = DefaultSupportedSecurityPolicies.ToArray();
        }

        if (configuration.UserTokenPolicies is not { Count: > 0 })
        {
            configuration.UserTokenPolicies = SupportedTokenPolicies.ToArray();
        }
    }
}