using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Entity.Api;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Hoeyer.OpcUa.Server.Test.Fixtures.Application;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Test.Application;

[ApplicationServiceCollectionGenerator]
public class EntityBrowserTest(ApplicationServiceCollectionFixture applicationServices)
{
    private readonly IEntityBrowser _entityBrowser = applicationServices.Browser;
    private readonly IEntityNodeHandle _node = applicationServices.HandleManager.EntityHandle;
    private readonly EntityNode _entity = applicationServices.EntityNode;
    
    [Test]
    public async Task WhenBrowsingEntity_BrowseIsSuccess()
    {
        var continuation = CreateContinuationPoint(_node.Value);
        var browseResult = _entityBrowser.Browse(continuation, _node);
        await Assert.That(browseResult.IsSuccess).IsTrue();
    }

    [Test]
    public async Task WhenBrowsingEntity_GetsAsManyResultsAsProperties()
    {
        var continuation = CreateContinuationPoint(_node.Value);
        var browseResult = _entityBrowser.Browse(continuation, _node);
        await Assert.That(browseResult.Value.Count()).IsEqualTo(_entity.PropertyStates.Count);
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