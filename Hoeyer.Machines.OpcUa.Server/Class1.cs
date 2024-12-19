using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using Opc.Ua.Configuration;

public class SimpleOpcUaServer
{
    private ApplicationInstance _application;
    private StandardServer _server;
    private ServerSystemContext _systemContext;
    public readonly ApplicationConfiguration Configuration = CreateApplicationConfiguration();
    private string _opcServerUrl = "opc.tcp://localhost:4840/freeopcua/server/"; 
    private string _machineStateNodeId = "ns=2;s=MachineStateSnapshot"; 

    public SimpleOpcUaServer()
    {
        _application = new ApplicationInstance
        {
            ApplicationName = "SimpleOpcUaServer",
            ApplicationType = ApplicationType.Server
        };
        

        _application.ApplicationConfiguration = Configuration;

        _server = new StandardServer();
    }

    public void Start()
    {
        var endpointDescription = new EndpointDescription
        {
            EndpointUrl = "opc.tcp://localhost:4840",
            SecurityMode = MessageSecurityMode.None,
            SecurityPolicyUri = SecurityPolicies.None
        };

        EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(_application.ApplicationConfiguration);
        
        var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
    }

    public void PrintNodes(Node node)
    {
        // Print node details, including namespace and index and its children (if any)
        PrintNodeDetails(node, "", 0);
    }

    private void PrintNodeDetails(Node node, string parentNamespace, int index)
    {
        string fullNamespace = parentNamespace + ":" + index.ToString();
        Console.WriteLine($"Node: {node.NodeId}, Namespace: {fullNamespace}, Index: {index}");
    }

    public static ApplicationConfiguration CreateApplicationConfiguration()
    {
        return new ApplicationConfiguration()
        {
            ApplicationName = "OpcUaServerApp",
            ApplicationType = ApplicationType.Server,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier(),
                AutoAcceptUntrustedCertificates = true, // Allow untrusted certificates
                AddAppCertToTrustedStore = true
            },
            ServerConfiguration = new ServerConfiguration
            {
                MaxSubscriptionCount = 1000,
                MinSubscriptionLifetime = 10000,
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            TraceConfiguration = new TraceConfiguration()
        };

    }
}

