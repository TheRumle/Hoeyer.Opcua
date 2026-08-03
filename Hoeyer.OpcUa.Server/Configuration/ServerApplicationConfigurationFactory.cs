using System;
using System.Collections.Generic;
using System.Linq;
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
    private static readonly IEnumerable<ServerSecurityPolicy> DefaultSupportedSecurityPolicies =
        new List<(string Uri, MessageSecurityMode Mode)>
        {
            ("http://opcfoundation.org/UA/SecurityPolicy#None", MessageSecurityMode.None),
            //("http://opcfoundation.org/UA/SecurityPolicy#Basic256", MessageSecurityMode.Sign),
            //("http://opcfoundation.org/UA/SecurityPolicy#Basic256", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Basic256", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256", MessageSecurityMode.Sign),
            //("http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Basic128Rsa15", MessageSecurityMode.Sign),
            //("http://opcfoundation.org/UA/SecurityPolicy#Basic128Rsa15", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Basic128Rsa15", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Aes128_Sha256_RsaOaep", MessageSecurityMode.Sign),
            //("http://opcfoundation.org/UA/SecurityPolicy#Aes128_Sha256_RsaOaep", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Aes128_Sha256_RsaOaep", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Aes256_Sha256_RsaPss", MessageSecurityMode.Sign),
            //("http://opcfoundation.org/UA/SecurityPolicy#Aes256_Sha256_RsaPss", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Aes256_Sha256_RsaPss", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Https", MessageSecurityMode.Sign),
            //("http://opcfoundation.org/UA/SecurityPolicy#Https", MessageSecurityMode.SignAndEncrypt),
            //("http://opcfoundation.org/UA/SecurityPolicy#Https", MessageSecurityMode.SignAndEncrypt)
        }.Select(e => new ServerSecurityPolicy
        {
            SecurityPolicyUri = e.Uri,
            SecurityMode = e.Mode
        }).ToList();

    private static readonly IReadOnlyCollection<UserTokenPolicy> SupportedTokenPolicies = new List<UserTokenPolicy>
    {
        new(UserTokenType.Anonymous),
        new(UserTokenType.UserName),
        new(UserTokenType.Certificate),
        new(UserTokenType.IssuedToken)
    };

    private static readonly ServerSecurityPolicyCollection SecurityPolicyCollection =
        new(DefaultSupportedSecurityPolicies);

    public ApplicationConfiguration CreateServerConfiguration()
    {
        try
        {
            var template = templateFactory.CreateTemplate();
            var templated = new ApplicationConfiguration(template.Configuration);
            templated.ServerConfiguration ??= new ServerConfiguration();
            templated.SecurityConfiguration ??= new SecurityConfiguration();
            additionalConfigure?.Invoke(templated);
            return ConfigureRoot(templated);
        }
        catch (Exception e)
        {
            throw new OpcUaEntityServerException("The created configuration was invalid", e);
        }
    }

    private ApplicationConfiguration ConfigureRoot(ApplicationConfiguration config)
    {
        config.ApplicationType = ApplicationType.Server;
        SetupServerConfiguration(config, targetServerSetup.Endpoints);
        return config;
    }


    private ServerConfiguration SetupServerConfiguration(ApplicationConfiguration appConfig, ISet<Uri> endpoints)
    {
        var configuration = appConfig.ServerConfiguration;
        configuration.BaseAddresses ??= new StringCollection(endpoints.Select(e => e.AbsoluteUri));
        if (configuration.BaseAddresses is not { Count: > 0 })
        {
            configuration.BaseAddresses = new StringCollection(endpoints.Select(e => e.AbsoluteUri));
        }

        if (configuration.SecurityPolicies is not { Count: > 0 })
        {
            configuration.SecurityPolicies = SecurityPolicyCollection;
        }

        if (configuration.UserTokenPolicies is not { Count: > 0 })
        {
            configuration.UserTokenPolicies = new UserTokenPolicyCollection(SupportedTokenPolicies);
        }

        return configuration;
    }
}