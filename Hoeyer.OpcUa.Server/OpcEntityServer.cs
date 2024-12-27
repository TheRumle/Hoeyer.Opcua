using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Server.Entity;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server;

public sealed class OpcEntityServer : StandardServer
{
    private readonly IEntityNodeManagerFactory _factory;
    public readonly Uri RootUri;

    public OpcEntityServer(IEntityNodeManagerFactory factory, string applicationUri)
    {
        _factory = factory;
        RootUri = new Uri(applicationUri);
    }


    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
    {
        // Define your dynamic namespace URI. This should be unique to your application.
        return new MasterNodeManager(server,
            configuration,
            RootUri.ToString(), 
            _factory.GetEntityManagers(server, configuration).ToArray());
    }
    
    public IEnumerable<Uri> EndPoints => CurrentInstance.EndpointAddresses;

}