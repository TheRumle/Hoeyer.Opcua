using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Opc.Ua;
using Opc.Ua.Security.Certificates;

namespace Hoeyer.OpcUa.Server.Configuration;

public class ApplicationConfigurationFactory(IOptions<OpcUaServerApplicationOptions> options) : IApplicationConfigurationFactory
{
    /// <inheritdoc />
    public string ApplicationName { get; } = options.Value.ApplicationName;

    /// <inheritdoc />
    public Uri ApplicationUri { get; } = new(options.Value.ApplicationUri);
    public string ApplicationUriString => ApplicationUri.ToString();

    private ServerSecurityPolicyCollection ServerSecurityPolicyCollection = new()
    {
        new ServerSecurityPolicy { SecurityMode = MessageSecurityMode.None, SecurityPolicyUri = SecurityPolicies.None },
    };

    private UserTokenPolicyCollection UserTokenPolicyCollection = new()
    {
        new UserTokenPolicy { TokenType = UserTokenType.Anonymous },
    };
    
    

    public ApplicationConfiguration CreateServerConfiguration(string subject, params string[] legalDomains)
    {
        var config = CreateApplicationConfiguration();
        AssignApplicationInstanceCertificate(config, subject, legalDomains);
        AssignDiscoveryConfiguration(config);
        config.Validate(ApplicationType.Server).Wait();
        return config;
    }

    private void AssignDiscoveryConfiguration(ApplicationConfiguration config)
    {
        var discoveryConfig = new DiscoveryServerConfiguration()
        {
            BaseAddresses = [ApplicationUri.ToString()],
            SecurityPolicies = ServerSecurityPolicyCollection,
            ServerNames = { ApplicationName },
        };
        config.DiscoveryServerConfiguration = discoveryConfig;
    }

    private ApplicationConfiguration CreateApplicationConfiguration()
    {
        ApplicationConfiguration config = new ApplicationConfiguration
        {
            ApplicationUri = ApplicationUriString,
            ApplicationName = ApplicationName,
            ApplicationType = ApplicationType.Server,
            CertificateValidator = new CertificateValidator(),
            ServerConfiguration = new ServerConfiguration
            {
                BaseAddresses = new StringCollection { ApplicationUriString },
                SecurityPolicies = ServerSecurityPolicyCollection,
                UserTokenPolicies = UserTokenPolicyCollection,
                RegistrationEndpoint = new EndpointDescription(ApplicationUriString)
            },
            DisableHiResClock = false,
        };

        return config;
    }

    private void AssignApplicationInstanceCertificate(ApplicationConfiguration config, string subject, string[] legalDomains)
    {
        ISet<string> domains = new HashSet<string>(["localhost", "127.0.0.1", ..legalDomains]);
        ICertificateBuilder appCert = CertificateFactory.CreateCertificate(ApplicationUriString, ApplicationName, subject, domains.ToList() );
        var certificate = appCert.CreateForRSA();
       

        // Assign the created certificate to the configuration
        config.SecurityConfiguration = new SecurityConfiguration
        {
            ApplicationCertificate = new CertificateIdentifier
            {
                Certificate = certificate
            },
            TrustedPeerCertificates = new CertificateTrustList(),
            RejectedCertificateStore = new CertificateTrustList()
        };
        config.TransportQuotas = new TransportQuotas();

        // Set up certificate validation to accept all certificates
        config.CertificateValidator.CertificateValidation += (sender, eventArgs) =>
        {
            eventArgs.Accept = true;
        };  
    }
}