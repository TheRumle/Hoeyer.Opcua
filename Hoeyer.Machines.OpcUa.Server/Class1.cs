using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Opc.Ua.Configuration;

public record OpcUaServerOptions
{
    public int Port { get; set; }
    public string OpcServerUrl { get; set; }
    public string ServerName { get; set; }

}

public class EntityOpcuaServer
{
    private ApplicationInstance _application;
    public StandardServer _server;
    private ServerSystemContext _systemContext;
    public readonly ApplicationConfiguration Configuration;
    private readonly string _opcServerUrl; 
    private string _machineStateNodeId = "ns=2;s=MachineStateSnapshot";
    private readonly OpcUaServerOptions options;

    public EntityOpcuaServer(IOptions<OpcUaServerOptions> options)
    {
        this.options = options.Value;
        Configuration = CreateApplicationConfiguration();
        _opcServerUrl = options.Value.OpcServerUrl;
        _server = CreateServer();
    }

    private StandardServer CreateServer()
    {
        _application = new ApplicationInstance
        {
            ApplicationName = options.ServerName,
            ApplicationType = ApplicationType.Server
        };
        

        _application.ApplicationConfiguration = Configuration;

        _server = new StandardServer();
        return _server;
    }


 
    public Uri Start()
    {
        var endpointDescription = new EndpointDescription
        {
            EndpointUrl = $"opc.tcp://localhost:{options.Port}",
            SecurityMode = MessageSecurityMode.None,
            SecurityPolicyUri = SecurityPolicies.None
        };

        EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(_application.ApplicationConfiguration);
        var endpoint = new ConfiguredEndpoint(new(), endpointDescription, endpointConfiguration);
        _server.Start(this.Configuration);
        return endpoint.EndpointUrl;
    }

    public void Stop() => _server.Stop();

    public ApplicationConfiguration CreateApplicationConfiguration()
    {
        var securityPolicy = new ServerSecurityPolicy();
        securityPolicy.SecurityMode = MessageSecurityMode.None;
        var a = new ApplicationConfiguration
        {
            ApplicationName = options.ServerName,
            ApplicationType = ApplicationType.Server,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier(),
                AutoAcceptUntrustedCertificates = true, 
                AddAppCertToTrustedStore = true
            },
            ServerConfiguration = new ServerConfiguration
            {
                MaxSubscriptionCount = 1000,
                MinSubscriptionLifetime = 10000,
                SecurityPolicies = new ServerSecurityPolicyCollection()
                {
                   securityPolicy
                }
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            TraceConfiguration = new TraceConfiguration()
        };

        return a;
    }
}