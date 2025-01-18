using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Nodes;
using Hoeyer.OpcUa.Server.Entity;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class OpcEntityServer : StandardServer
{
    private readonly IEnumerable<IEntityNodeCreator> _entityObjectCreators;
    public readonly Uri RootUri;


    public OpcEntityServer(IEnumerable<IEntityNodeCreator> entityObjectCreators, string applicationUri)
    {
        _entityObjectCreators = entityObjectCreators;
        RootUri = new Uri(applicationUri);
    }

    public IReadOnlyList<EntityNodeManager> EntityNodesManagers { get; private set; }
    public IEnumerable<NodeState> EntityNodes => EntityNodesManagers.Select(e => e.EntityNode);
    public IServerInternal Server => ServerInternal;


    public IEnumerable<Uri> EndPoints => CurrentInstance.EndpointAddresses;

    /// <inheritdoc />
    protected override void OnUpdateConfiguration(ApplicationConfiguration configuration)
    {
        configuration.ApplicationUri = RootUri.ToString();
        base.OnUpdateConfiguration(configuration);
    }


    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server,
        ApplicationConfiguration configuration)
    {
        EntityNodesManagers = _entityObjectCreators
            .Select(creator => new EntityNodeManager(creator, server, configuration)).ToList();
        return new MasterNodeManager(server,
            configuration,
            RootUri.ToString(),
            EntityNodesManagers
                .Select(INodeManager (m) => m)
                .Union(NodeManagerFactories != null
                    ? NodeManagerFactories.Select(f => f.Create(server, configuration))
                    : [])
                .ToArray());
    }
}