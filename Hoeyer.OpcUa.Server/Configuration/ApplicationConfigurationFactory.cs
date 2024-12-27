using System;
using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Security.Certificates;

namespace Hoeyer.OpcUa.Server.Configuration;

public class ApplicationConfigurationFactory(string applicationName, string baseAddress) : IApplicationConfigurationFactory
{
    /// <inheritdoc />
    public string ApplicationName { get; } = applicationName;

    /// <inheritdoc />
    public Uri BaseAddress { get; } = new(baseAddress);

    public ApplicationConfiguration CreateServerConfiguration(string subject, params string[] legalDomains)
    {
        var config = CreateApplicationConfiguration();
        AssignApplicationInstanceCertificate(config, subject, legalDomains);
        config.Validate(ApplicationType.Server).Wait();
        return config;
    }

    private ApplicationConfiguration CreateApplicationConfiguration()
    {
        ApplicationConfiguration config = new ApplicationConfiguration
        {
            ApplicationName = applicationName,
            ApplicationType = ApplicationType.Server,
            CertificateValidator = new CertificateValidator(),
            ServerConfiguration = new ServerConfiguration
            {
                BaseAddresses = new StringCollection { baseAddress },
                SecurityPolicies = new ServerSecurityPolicyCollection
                {
                    new ServerSecurityPolicy { SecurityMode = MessageSecurityMode.None, SecurityPolicyUri = SecurityPolicies.None },
                },
                UserTokenPolicies = new UserTokenPolicyCollection
                {
                    new UserTokenPolicy { TokenType = UserTokenType.Anonymous },
                },
            },
            DisableHiResClock = false,
        };

        return config;
    }

    private void AssignApplicationInstanceCertificate(ApplicationConfiguration config, string subject, string[] legalDomains)
    {
        IList<string> domains = ["localhost", "127.0.0.1", ..legalDomains];
        ICertificateBuilder appCert = CertificateFactory.CreateCertificate(baseAddress, applicationName, subject, domains );
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