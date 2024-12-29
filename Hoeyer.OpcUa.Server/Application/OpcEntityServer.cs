using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Server.Entity;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class OpcEntityServer : StandardServer
{
    private readonly IEnumerable<IEntityObjectStateCreator> _entityObjectCreators;
    public readonly Uri RootUri;
    public IServerInternal Server => ServerInternal;

    public OpcEntityServer(IEnumerable<IEntityObjectStateCreator> entityObjectCreators, string applicationUri)
    {
        this._entityObjectCreators = entityObjectCreators;
        RootUri = new Uri(applicationUri);
    }


    
    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
    {
        return new MasterNodeManager(server,
            configuration,
            RootUri.ToString(), 
            _entityObjectCreators.Select(INodeManager (e) => new SingletonEntityNodeManager(e, server, configuration)).ToArray());
    }
    
    public IEnumerable<Uri> EndPoints => CurrentInstance.EndpointAddresses;

}