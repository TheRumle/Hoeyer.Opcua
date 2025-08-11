using System.Diagnostics.CodeAnalysis;
using Hoeyer.Common.Extensions.Async;
using Hoeyer.Common.Extensions.Collection;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using JetBrains.Annotations;
using Opc.Ua;
using AgentBrowseException = Hoeyer.OpcUa.Client.Api.Browsing.Exceptions.AgentBrowseException;

namespace Hoeyer.OpcUa.EndToEndTest;

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
    public async Task WhenTraversingWithNoMatch_ThrowsAgentBrowseException(CancellationToken token)
    {
        await Assert.ThrowsAsync<AgentBrowseException>(
            fixture.ExecuteWithSessionAsync((session, strategy)
                => strategy.TraverseUntil(session, ObjectIds.RootFolder, e => false, token)));
    }

    [Test]
    [NotInParallel(nameof(WhenTraversingWithMatch_DoesNotThrowNotFound))]
    [MethodDataSource(typeof(SelectedNodeIds), nameof(SelectedNodeIds.PresentObjects))]
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
            await traversalResult.ThenAsync(nodeReferences => nodeReferences.Select(n => n.NodeId));

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
    [MethodDataSource(typeof(SelectedNodeIds), nameof(SelectedNodeIds.PresentObjects))]
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
}