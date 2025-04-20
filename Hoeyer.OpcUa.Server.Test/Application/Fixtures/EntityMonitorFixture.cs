using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Application.Handle;

namespace Hoeyer.OpcUa.Server.Test.Application.Fixtures;
 

public sealed class EntityMonitorFixture
{
    public IEntityNode EntityNode { get; set; }
    public EntityMonitorFixture(IEntityNodeStructureFactory creator)
    {
        var managedNode = creator.Create(2); 
        this.EntityNode = managedNode;
        HandleManager = new EntityHandler(managedNode);
    }

    public IEntityHandleManager HandleManager { get; set; }
}