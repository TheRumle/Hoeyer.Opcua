using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Fixtures;

public sealed record ApplicationServiceCollectionFixture
{
    public readonly IEntityBrowser Browser;
    public readonly EntityNode EntityNode;
    public readonly IEntityHandleManager HandleManager;
    public readonly IEntityNodeCreator NodeCreator;
    public readonly IPropertyReader PropertyReader;
    public readonly IEntityReader Reader;
    public readonly IReferenceLinker ReferenceLinker;
    public readonly IEntityWriter Writer;
    public string EntityName => EntityNode.Entity.BrowseName.Name;
    public IEntityNodeHandle EntityHandle => HandleManager.EntityHandle;
    public IEnumerable<IEntityNodeHandle> PropertyHandles => HandleManager.PropertyHandles;
    public Dictionary<NodeId, PropertyState> PropertyStates => EntityNode.PropertyStates;
    
    public ApplicationServiceCollectionFixture(IEntityNodeCreator NodeCreator)
    {
        this.NodeCreator = NodeCreator;
        var managedNode = NodeCreator.CreateEntityOpcUaNode(2);
        PropertyReader = new PropertyReader();
        EntityNode = managedNode;
        HandleManager = new EntityHandleManager(managedNode);
        Writer = new EntityWriter(managedNode);
        Browser = new EntityBrowser(managedNode);
        Reader = new EntityReader(managedNode, PropertyReader);
        ReferenceLinker = new EntityReferenceLinker(managedNode);
    }

    public override string ToString()
    {
        return $"ServiceCollectionFixture, {EntityNode.Entity.DisplayName.ToString()}";
    }
}