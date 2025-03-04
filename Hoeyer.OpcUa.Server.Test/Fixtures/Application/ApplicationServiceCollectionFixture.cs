using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Management;

namespace Hoeyer.OpcUa.Server.Test.Fixtures.Application;

public sealed record ApplicationServiceCollectionFixture
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"ServiceCollectionFixture, {EntityNode.Entity.DisplayName.ToString()}";
    }

    public readonly IEntityBrowser Browser;
    public readonly EntityNode EntityNode;
    public readonly IEntityHandleManager HandleManager;
    public readonly IEntityNodeCreator NodeCreator;
    public readonly IPropertyReader PropertyReader;
    public readonly IEntityReader Reader;
    public readonly IReferenceLinker ReferenceLinker;
    public readonly IEntityWriter Writer;

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
}