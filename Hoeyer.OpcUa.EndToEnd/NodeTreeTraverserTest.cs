using System.Diagnostics.CodeAnalysis;
using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using JetBrains.Annotations;
using Opc.Ua;
using EntityBrowseException = Hoeyer.OpcUa.Client.Api.Browsing.Exceptions.EntityBrowseException;

namespace Hoeyer.OpcUa.EndToEndTest;

[TestSubject(typeof(INodeTreeTraverser))]
public abstract class NodeTreeTraverserTest<T>(ApplicationFixture<T> fixture)  where T : INodeTreeTraverser
{
    /// <inheritdoc />
    public override string ToString()
    {
        return fixture.TestedService.GetType().Name;
    }

    public static IEnumerable<Func<NodeId>> PresentObjects()
    {
        IEnumerable<NodeId> ids =
        [
            ObjectIds.Dictionaries, ObjectIds.Aliases, ObjectIds.Locations, ObjectIds.Quantities, ObjectIds.Resources
        ];
        return ids.SelectFunc();
    } 
    
    [Test]
    public async Task WhenTraversingWithNoMatch_ThrowsEntityBrowseException()
    {
        await Assert.ThrowsAsync<EntityBrowseException>(
            fixture.ExecuteWithSessionAsync((session, strategy) => strategy.TraverseUntil(session, ObjectIds.RootFolder, e => false)));
    }
    
    [Test]
    [NotInParallel("WithMatch")]
    [MethodDataSource<SelectedNodeIds>(nameof(PresentObjects))]
    public async Task WhenTraversingWithMatch_DoesNotThrowNotFound(NodeId id)
    {
        var result = await fixture.ExecuteWithSessionAsync((session, strategy) =>
            strategy.TraverseUntil(session, ObjectIds.RootFolder, e => e.NodeId.Equals(id)));

        await Assert.That(result).IsNotDefault();

    }
    
    [Test]
    [MethodDataSource<SelectedNodeIds>(nameof(PresentObjects))]
    [NotInParallel("SelfLookup")]
    public async Task WhenLookingForSelf_DoesNotThrowNotFound(NodeId id)
    {
        var result = await fixture.ExecuteWithSessionAsync((session, strategy) =>
            strategy.TraverseUntil(session, id, e => e.NodeId.Equals(id)));

        await Assert.That(result).IsNotDefault();

    }
    
    [Test]
    [SuppressMessage("Maintainability", "S108", Justification = "The test must consume the traversal results")]
    public async Task WhenTraversingFromRoot_DoesNotLoopForever()
    {
        var value = fixture.TestedService;
        var session = await fixture.CreateSession();
        await foreach (var a in value.TraverseFrom(ObjectIds.RootFolder, session, CancellationToken.None))
        {}
    }
}