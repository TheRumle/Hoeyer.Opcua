using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application.Fixtures;

public sealed record EntityBrowserFixture
{
    public readonly IEntityNode EntityNode;
    public readonly IEntityHandleManager HandleManager;
    public string EntityName => EntityNode.BaseObject.BrowseName.Name;
    public IEntityNodeHandle EntityHandle => HandleManager.EntityHandle;
    public Dictionary<NodeId, PropertyState> PropertyStates => EntityNode.PropertyStates;
    
    public EntityBrowserFixture(IEntityNodeCreator nodeCreator)
    {
        var managedNode = nodeCreator.CreateEntityOpcUaNode(2);
        EntityNode = managedNode;
        HandleManager = new EntityHandleManager(managedNode);
    }

    public override string ToString()
    {
        return $"{EntityNode.BaseObject.DisplayName.ToString()}";
    }
}