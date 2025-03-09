using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Hoeyer.OpcUa.Server.Test.Application.Generators;
using Hoeyer.OpcUa.Server.Test.Fixtures;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Test.Application;

[EntityBrowserFixtureGenerator]
public class EntityBrowserTest(EntityBrowserFixture fixture)
{
    private readonly IEntityBrowser _entityBrowser = new EntityBrowser(fixture.EntityNode);
    private readonly IEntityNodeHandle _entityHandle = fixture.HandleManager.EntityHandle;
    private readonly EntityNode _entity = fixture.EntityNode;
    
    [Test]
    public async Task WhenBrowsingEntity_BrowseIsSuccess()
    {
        var continuation = CreateContinuationPoint(_entity.Entity);
        var browseResult = _entityBrowser.Browse(continuation, fixture.EntityHandle);
        await Assert.That(browseResult.IsSuccess).IsTrue();
    }

    [Test]
    public async Task WhenBrowsingEntity_GetsAsManyResultsAsProperties()
    {
        var continuation = CreateContinuationPoint(_entityHandle.Value);
        var browseResult = _entityBrowser.Browse(continuation, _entityHandle);
        await Assert.That(browseResult.Value.RelatedEntities.Count).IsEqualTo(_entity.PropertyStates.Count);
    }

    private static ContinuationPoint CreateContinuationPoint(NodeState node)
    {
        return new ContinuationPoint()
        {
            BrowseDirection = BrowseDirection.Forward,
            Data = null,
            Id = Guid.NewGuid(),
            IncludeSubtypes = true,
            Index = 0,
            Manager = null,
            MaxResultsToReturn = uint.MaxValue,
            NodeToBrowse = node,
            ReferenceTypeId = ReferenceTypes.References
        };
    }

}