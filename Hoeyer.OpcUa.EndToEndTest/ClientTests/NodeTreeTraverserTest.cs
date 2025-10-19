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
public abstract class NodeTreeTraverserTest(
    ApplicationFixture fixture,
    string classUnderTest,
    Func<INodeTreeTraverser> getTraversal)
{
    /// <inheritdoc />
    public override string ToString() => classUnderTest;

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
        var action = async () => await fixture.ExecuteActionAsync(async (session, provider) =>
        {
            var strategy = getTraversal.Invoke();
            await strategy.TraverseUntil(session, ObjectIds.RootFolder, e => false, token);
        });
        await Assert.ThrowsAsync<EntityBrowseException>(action);
    }

    [Test]
    [NotInParallel(nameof(WhenTraversingWithMatch_DoesNotThrowNotFound))]
    [InstanceMethodDataSource(nameof(PresentObjects))]
    [Timeout(10_0000)]
    public async Task WhenTraversingWithMatch_DoesNotThrowNotFound(NodeId id, CancellationToken token)
    {
        var result = await fixture.ExecuteFunctionAsync(async (session, provider) =>
        {
            var strategy = getTraversal.Invoke();
            return await strategy.TraverseUntil(session, ObjectIds.RootFolder, e => e.NodeId.Equals(id), token);
        });
        await Assert.That(result).IsNotNull();
    }

    [Test]
    [NotInParallel(nameof(WhenTraversing_DoesNotGiveDuplicateNodes))]
    [Timeout(10_0000)]
    public async Task WhenTraversing_DoesNotGiveDuplicateNodes(CancellationToken token)
    {
        var ids = await fixture
            .ExecuteFunctionAsync(async (session, provider) => await getTraversal.Invoke()
                .TraverseFrom(ObjectIds.RootFolder, session, token)
                .Collect())
            .ThenAsync(nodeReferences => nodeReferences.Select<ReferenceWithId, NodeId>(n => n.NodeId));

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
        var result = await fixture.ExecuteFunctionAsync(async (session, provider) =>
        {
            var strategy = getTraversal.Invoke();
            return await strategy.TraverseUntil(session, id, e => e.NodeId.Equals(id), token);
        });
        await Assert.That(result).IsNotNull();
    }

    [Test]
    [SuppressMessage("Maintainability", "S108", Justification = "The test must consume the traversal results")]
    [Timeout(10_0000)]
    public async Task WhenTraversingFromRoot_DoesNotLoopForever(CancellationToken ct)
    {
        var value = getTraversal();
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
        var traversalStrategy = getTraversal();
        var session = await fixture.CreateSession();
        await traversalStrategy
            .TraverseUntil(session,
                ObjectIds.RootFolder,
                node => browseNameCollection.EntityName.Equals(node.BrowseName.Name),
                token);
    }
}