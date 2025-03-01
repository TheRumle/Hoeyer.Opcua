using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.NodeManagement;

namespace Hoeyer.OpcUa.Server.Test.Fixtures.Application;

public sealed record ApplicationServiceCollectionFixture
{
    public readonly IEntityHandleManager HandleManager;
    public readonly IEntityModifier Modifier;
    public readonly IEntityBrowser Browser;
    public readonly IEntityReader Reader;
    public readonly IReferenceLinker ReferenceLinker;
    public readonly IEntityNodeCreator NodeCreator;
    public readonly EntityNode EntityNode;
    public readonly IPropertyReader PropertyReader;

    public ApplicationServiceCollectionFixture(IEntityNodeCreator NodeCreator)
    {
        this.NodeCreator = NodeCreator;
        var managedNode = NodeCreator.CreateEntityOpcUaNode(2);
        PropertyReader = new PropertyReader();
        EntityNode = managedNode;
        HandleManager = new EntityHandleManager(managedNode);
        Modifier = new EntityModifier(managedNode);
        Browser = new EntityBrowser(managedNode);
        Reader = new EntityReader(managedNode, PropertyReader);
        ReferenceLinker = new EntityReferenceLinker(managedNode);
    }

}