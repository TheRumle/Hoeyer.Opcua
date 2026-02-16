using System.Diagnostics.CodeAnalysis;
using Hoeyer.Common.Extensions.Async;
using Hoeyer.Common.Extensions.Collection;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Test.Api;
using JetBrains.Annotations;
using Opc.Ua;
using EntityBrowseException = Hoeyer.OpcUa.Client.Api.Browsing.Exceptions.EntityBrowseException;

namespace Hoeyer.OpcUa.Test.Client;

[TestSubject(typeof(INodeTreeTraverser))]
[TestSubject(typeof(ConcurrentBrowse))]
[Timeout(10_0000)]
[DependsOn<AdapterTest>]
public abstract class NodeTreeTraverserTest(
    ISimulationTestSession fixture,
    string classUnderTest,
    Func<INodeTreeTraverser> getTraversal)
{
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
    public async Task WhenTraversingWithNoMatch_ThrowsEntityBrowseException(CancellationToken token)
    {
        var action = async () => await fixture.ExecuteWithSession(async session =>
        {
            var strategy = getTraversal.Invoke();
            await strategy.TraverseUntil(session, ObjectIds.RootFolder, e => false, token);
        });
        await Assert.ThrowsAsync<EntityBrowseException>(action);
    }

    [Test]
    [InstanceMethodDataSource(nameof(PresentObjects))]
    public async Task WhenTraversingWithMatch_DoesNotThrowNotFound(NodeId id, CancellationToken token)
    {
        var result = await fixture.ExecuteWithSession(async session =>
        {
            var strategy = getTraversal.Invoke();
            return await strategy.TraverseUntil(session, ObjectIds.RootFolder, e => e.NodeId.Equals(id), token);
        });
        await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task WhenTraversing_DoesNotGiveDuplicateNodes(CancellationToken token)
    {
        var ids = await fixture
            .ExecuteWithSession(async session => await getTraversal.Invoke()
                .TraverseFrom(ObjectIds.RootFolder, session, token)
                .Collect())
            .ThenAsync(nodeReferences => nodeReferences.Select<ReferenceWithId, NodeId>(n => n.NodeId));

        List<IGrouping<NodeId, NodeId>> duplicates = ids.GroupBy(x => x).Where(g => g.Count() > 1).ToList();
        using (Assert.Multiple())
        {
            foreach (IGrouping<NodeId, NodeId> duplicate in duplicates)
            {
                await Assert.That(duplicate.Count()).IsLessThan(1).Because(duplicate.Key +
                                                                           " should only be returned once but was returned " +
                                                                           duplicates.Count + " times");
            }
        }
    }

    [Test]
    [InstanceMethodDataSource(nameof(PresentObjects))]
    public async Task WhenLookingForSelf_DoesNotThrowNotFound(NodeId id, CancellationToken token)
    {
        var result = await fixture.ExecuteWithSession(async session =>
        {
            var strategy = getTraversal.Invoke();
            return await strategy.TraverseUntil(session, id, e => e.NodeId.Equals(id), token);
        });
        await Assert.That(result).IsNotNull();
    }

    [Test]
    [SuppressMessage("Maintainability", "S108", Justification = "The test must consume the traversal results")]
    public async Task WhenTraversingFromRoot_DoesNotLoopForever(CancellationToken ct)
    {
        var value = getTraversal();
        var session = await fixture.GetOrCreateSession();
        await foreach (ReferenceWithId _ in value.TraverseFrom(ObjectIds.RootFolder, session, ct))
        {
        }
    }

    private IEnumerable<IBrowseNameCollection> ConstructBrowseNameCollections() =>
        fixture.GetService<IEnumerable<IBrowseNameCollection>>();

    [Test]
    [DisplayName("Can find entity root for $browseNameCollection")]
    public async Task CanFindReferencesForAllNodes(CancellationToken token)
    {
        IEnumerable<IBrowseNameCollection> browseNameCollections = ConstructBrowseNameCollections();
        using var assertion = Assert.Multiple();

        foreach (var browseNameCollection in browseNameCollections)
        {
            var traversalStrategy = getTraversal();
            var session = await fixture.GetOrCreateSession();
            await traversalStrategy
                .TraverseUntil(session,
                    ObjectIds.RootFolder,
                    node => browseNameCollection.EntityName.Equals(node.BrowseName.Name),
                    token);
        }
    }
}