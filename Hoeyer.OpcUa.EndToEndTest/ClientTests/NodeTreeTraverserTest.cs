using System.Diagnostics.CodeAnalysis;
using Hoeyer.Common.Extensions.Async;
using Hoeyer.Common.Extensions.Collection;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using JetBrains.Annotations;
using Opc.Ua;
using EntityBrowseException = Hoeyer.OpcUa.Client.Api.Browsing.Exceptions.EntityBrowseException;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

[TestSubject(typeof(INodeTreeTraverser))]
[TestSubject(typeof(ConcurrentBrowse))]
public abstract class NodeTreeTraverserTest<T>(ApplicationFixture<T> fixture) where T : INodeTreeTraverser
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
    [Timeout(10_0000)]
    public async Task WhenTraversingWithNoMatch_ThrowsEntityBrowseException(CancellationToken token)
    {
        await Assert.ThrowsAsync<EntityBrowseException>(() => fixture.ExecuteWithSessionAsync((session, strategy) =>
            strategy.TraverseUntil(session, ObjectIds.RootFolder, e => false, token)));
    }

    [Test]
    [NotInParallel(nameof(WhenTraversingWithMatch_DoesNotThrowNotFound))]
    [InstanceMethodDataSource(nameof(PresentObjects))]
    [Timeout(10_0000)]
    public async Task WhenTraversingWithMatch_DoesNotThrowNotFound(NodeId id, CancellationToken token)
    {
        var result = await fixture.ExecuteWithSessionAsync((session, strategy) =>
            strategy.TraverseUntil(session, ObjectIds.RootFolder, e => e.NodeId.Equals(id), token));

        await Assert.That(result).IsNotDefault();
    }

    [Test]
    [NotInParallel(nameof(WhenTraversing_DoesNotGiveDuplicateNodes))]
    [Timeout(10_0000)]
    public async Task WhenTraversing_DoesNotGiveDuplicateNodes(CancellationToken token)
    {
        Task<IEnumerable<ReferenceWithId>> traversalResult = fixture.ExecuteWithSessionAsync((session, strategy)
            => strategy.TraverseFrom(ObjectIds.RootFolder, session, token).Collect());

        IEnumerable<NodeId> ids =
            await traversalResult.ThenAsync(nodeReferences =>
                nodeReferences.Select<ReferenceWithId, NodeId>(n => n.NodeId));

        List<IGrouping<NodeId, NodeId>> duplicates = ids.GroupBy(x => x).Where(g => g.Count() > 1).ToList();
        using (Assert.Multiple())
        {
            foreach (IGrouping<NodeId, NodeId> duplicate in duplicates)
            {
                await Assert.That(duplicate.Count()).IsLessThan(2).Because(duplicate.Key +
                                                                           " should only be returned once but was returned " +
                                                                           duplicates.Count + " times");
            }
        }
    }

    [Test]
    [InstanceMethodDataSource(nameof(PresentObjects))]
    [NotInParallel(nameof(WhenLookingForSelf_DoesNotThrowNotFound))]
    [Timeout(10_0000)]
    public async Task WhenLookingForSelf_DoesNotThrowNotFound(NodeId id, CancellationToken token)
    {
        var result = await fixture.ExecuteWithSessionAsync((session, strategy) =>
            strategy.TraverseUntil(session, id, e => e.NodeId.Equals(id), token));

        await Assert.That(result).IsNotDefault();
    }

    [Test]
    [SuppressMessage("Maintainability", "S108", Justification = "The test must consume the traversal results")]
    [Timeout(10_0000)]
    public async Task WhenTraversingFromRoot_DoesNotLoopForever(CancellationToken ct)
    {
        var value = fixture.TestedService;
        var session = await fixture.CreateSession();
        await foreach (ReferenceWithId _ in value.TraverseFrom(ObjectIds.RootFolder, session, ct))
        {
        }
    }

    [Test]
    [BrowseNameCollection]
    [DisplayName("Can find entity root for $browseNameCollection")]
    public async Task CanFindReferencesForAllNodes(BrowseNameCollection browseNameCollection, CancellationToken token)
    {
        var traversalStrategy = fixture.TestedService;
        var session = await fixture.CreateSession();
        await traversalStrategy
            .TraverseUntil(session,
                ObjectIds.RootFolder,
                node => browseNameCollection.EntityName.Equals(node.BrowseName.Name),
                token);
    }
}