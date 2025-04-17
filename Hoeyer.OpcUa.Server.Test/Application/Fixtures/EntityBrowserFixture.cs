using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Application;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application.Fixtures;

public sealed record EntityBrowserFixture
{
    public readonly IEntityNode EntityNode;
    public readonly IEntityHandleManager HandleManager;

    public EntityBrowserFixture(IEntityNodeStructureFactory nodeCreator)
    {
        var managedNode = nodeCreator.Create(2);
        EntityNode = managedNode;
        HandleManager = new EntityHandleManager(managedNode);
    }

    public string EntityName => EntityNode.BaseObject.BrowseName.Name;
    public IEntityNodeHandle EntityHandle => HandleManager.EntityHandle;
    public Dictionary<NodeId, PropertyState> PropertyStates => EntityNode.PropertyStates;

    public override string ToString()
    {
        return $"{EntityNode.BaseObject.DisplayName.ToString()}";
    }
}