using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

internal static class ServerApplicationConfigurationFactory
{
    public static readonly IEnumerable<ServerSecurityPolicy> DefaultSupportedSecurityPolicies =
        new List<(string Uri, MessageSecurityMode Mode)>
        {
            ("http://opcfoundation.org/UA/SecurityPolicy#None", MessageSecurityMode.None)
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

    private static readonly ServerSecurityPolicyCollection SecurityPolicyCollection =
        new(DefaultSupportedSecurityPolicies);

    public static readonly IReadOnlyCollection<UserTokenPolicy> SupportedTokenPolicies = new List<UserTokenPolicy>
    {
        new(UserTokenType.Anonymous),
        new(UserTokenType.UserName),
        new(UserTokenType.Certificate),
        new(UserTokenType.IssuedToken)
    };

    /// <inheritdoc />
    public static ApplicationConfiguration CreateServerConfiguration(OpcUaEntityServerSetup configuration)
    {
        var applicationConfiguration = CreateApplicationConfiguration(configuration);
        SetupDefaultValues(applicationConfiguration.ServerConfiguration, configuration);

        applicationConfiguration.SecurityConfiguration = CreateSecurityConfiguration(applicationConfiguration);
        var serverConfiguration = applicationConfiguration.ServerConfiguration;

        configuration.AdditionalConfiguration.Invoke(applicationConfiguration.ServerConfiguration);
        serverConfiguration.Validate();
        return applicationConfiguration;
    }

    private static SecurityConfiguration CreateSecurityConfiguration(ApplicationConfiguration configuration)
    {
        var securityConfiguration = new SecurityConfiguration
        {
            ApplicationCertificate = new CertificateIdentifier
            {
                Certificate = CreateCertificate(configuration)
            },
            TrustedPeerCertificates = new CertificateTrustList(),
            RejectedCertificateStore = new CertificateTrustList(),
            AutoAcceptUntrustedCertificates = true
        };

        // Set up certificate validation to accept all certificates
        configuration.CertificateValidator.CertificateValidation += (sender, eventArgs) => { eventArgs.Accept = true; };
        return securityConfiguration;
    }


    private static ApplicationConfiguration CreateApplicationConfiguration(OpcUaEntityServerSetup configuration)
    {
        var config = new ApplicationConfiguration
        {
            ApplicationUri = configuration.ApplicationNamespace.ToString(),
            ApplicationName = configuration.ApplicationName,
            ApplicationType = ApplicationType.Server,
            CertificateValidator = new CertificateValidator(),
            ServerConfiguration = new ServerConfiguration
            {
                BaseAddresses = new StringCollection(configuration.Endpoints.Select(e => e.AbsoluteUri)),
                SecurityPolicies = SecurityPolicyCollection,
                UserTokenPolicies = new UserTokenPolicyCollection(SupportedTokenPolicies)
            },
            DisableHiResClock = false,
            TransportQuotas = new TransportQuotas
            {
                OperationTimeout = 600000,
                MaxStringLength = 1048576,
                MaxByteStringLength = 1048576,
                MaxArrayLength = ushort.MaxValue,
                MaxMessageSize = 4194304,
                MaxBufferSize = ushort.MaxValue,
                ChannelLifetime = 300000,
                SecurityTokenLifetime = 3600000
            }
        };

        return config;
    }

    private static X509Certificate2 CreateCertificate(ApplicationConfiguration configuration)
    {
        return CertificateFactory
            .CreateCertificate(configuration.ApplicationUri,
                configuration.ApplicationName,
                null,
                null)
            .CreateForRSA();
    }


    private static void SetupDefaultValues(ServerConfiguration serverConfiguration,
        OpcUaEntityServerSetup opcUaEntityEntityServer)
    {
        serverConfiguration.MinRequestThreadCount = 5;
        serverConfiguration.MaxRequestThreadCount = 100;
        serverConfiguration.MaxQueuedRequestCount = 200;
        serverConfiguration.MaxSessionCount = 75;
        serverConfiguration.MinSessionTimeout = 10000;
        serverConfiguration.MaxSessionTimeout = 3600000;
        serverConfiguration.MaxBrowseContinuationPoints = 10;
        serverConfiguration.MaxQueryContinuationPoints = 10;
        serverConfiguration.MaxHistoryContinuationPoints = 100;
        serverConfiguration.MaxRequestAge = 600000;
        serverConfiguration.MinPublishingInterval = 50;
        serverConfiguration.MaxPublishingInterval = 3600000;
        serverConfiguration.PublishingResolution = 50;
        serverConfiguration.MaxSubscriptionLifetime = 3600000;
        serverConfiguration.MaxMessageQueueSize = 100;
        serverConfiguration.MaxNotificationQueueSize = 100;
        serverConfiguration.MaxNotificationsPerPublish = 1000;
        serverConfiguration.MinMetadataSamplingInterval = 1000;
        serverConfiguration.MaxRegistrationInterval = 0;
        serverConfiguration.MinSubscriptionLifetime = 10000;
        serverConfiguration.MaxPublishRequestCount = 20;
        serverConfiguration.MaxSubscriptionCount = 100;
        serverConfiguration.MaxEventQueueSize = 10000;
        serverConfiguration.MaxTrustListSize = 0;
        serverConfiguration.MultiCastDnsEnabled = false;
        serverConfiguration.NodeManagerSaveFile = opcUaEntityEntityServer.ServerId + ".Nodes.xml";
        serverConfiguration.ShutdownDelay = 5;
    }
}