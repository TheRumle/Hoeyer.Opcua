using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Security.Certificates;
using Opc.Ua.Server;

namespace Hoeyer.Machines.OpcUa.Server;

public class OpcUaLocalHostServerFactory
{

    /// <summary>
    /// The base address of the application.
    /// <example>opc.tcp://localhost:123</example> 
    /// </summary>
    public readonly string BaseAddress;
    public string BaseUrl => BaseAddress;
    
    private readonly ApplicationInstance _application;
    private readonly string _serverName;
    private readonly string _subject;
    public ApplicationConfiguration ApplicationConfiguration => _application.ApplicationConfiguration;

    /// <summary>
    /// </summary>
    /// <param name="serverName">The name of the server</param>
    /// <param name="port">The port the server should use</param>
    /// <param name="subject">The unique name identifying the subject to create certificates to.</param>
    /// See <see href="https://reference.opcfoundation.org/Core/Part6/v105/docs/6.2"/> for information about <paramref name="subject"/> and its further detail 
    public OpcUaLocalHostServerFactory(string serverName, string port, string subject)
    {
        _serverName = serverName;
        _subject = subject;
        BaseAddress = "opc.tcp://" + "localhost:" + port;
        _application = new ApplicationInstance
        {
            ApplicationName = serverName,
            ApplicationType = ApplicationType.Server
        };
        _application.ApplicationConfiguration = CreateApplicationConfiguration();
        _application.LoadApplicationConfiguration(false);
    }

    public OpcUaLocalHostServerFactory(string serverName, int port, string subject)
        : this(serverName, port.ToString(), subject)
    {
        
    }

    public (StandardServer server, ApplicationConfiguration applicationConfigurtation) CreateServer()
    {
        var server = new StandardServer();
        return (server, _application.ApplicationConfiguration);
    }


    private ApplicationConfiguration CreateApplicationConfiguration()
    {
        // Create CreateEndpoint new ApplicationConfiguration instance
        ApplicationConfiguration config = new ApplicationConfiguration
        {
            ApplicationName = _serverName,
            ApplicationType = ApplicationType.Server,
            CertificateValidator = new CertificateValidator(),
            ServerConfiguration = new ServerConfiguration
            {
                BaseAddresses = new StringCollection { BaseAddress },
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
        CreateApplicationInstanceCertificate(config);
        config.Validate(ApplicationType.Server).Wait();
        return config;
    }

    private void CreateApplicationInstanceCertificate(ApplicationConfiguration config)
    {
        IList<string> domains = ["localhost", "127.0.0.1"];
        ICertificateBuilder appCert = CertificateFactory.CreateCertificate(BaseAddress, _serverName, _subject, domains );
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