using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Hoeyer.OpcUa.Server.Test.Application.Fixtures;
using Hoeyer.OpcUa.Server.Test.Application.Generators;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Test.Application;

[EntityFixtureGenerator]
public class EntityBrowserTest(EntityReaderFixture fixture)
{
    private readonly IEntityBrowser _entityBrowser = new EntityBrowser(fixture.Node);
    private readonly IEntityNodeHandle _entityHandle = fixture.EntityHandle;
    private readonly IEntityNode _entity = fixture.Node;
    
    [Test]
    public async Task WhenBrowsingEntity_BrowseIsSuccess()
    {
        var continuation = CreateContinuationPoint(_entity.BaseObject);
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
        return new ContinuationPoint
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