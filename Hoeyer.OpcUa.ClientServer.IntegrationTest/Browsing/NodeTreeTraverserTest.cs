using System.Diagnostics.CodeAnalysis;
using Hoeyer.Common.Extensions.Async;
using Hoeyer.Common.Extensions.Collection;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Browsing.Exceptions;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.IntegrationTest.Configuration;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.IntegrationTest.Browsing;

[TestSubject(typeof(INodeTreeTraverser))]
[TestSubject(typeof(ConcurrentBrowse))]
[Timeout(10_0000)]
[DependsOn<SessionConnectionTest>]
[NotInParallel]
public abstract class NodeTreeTraverserTest
{
    protected abstract INodeTreeTraverser TestedService { get; }

    private Task<IEntitySession> OpenedSession => field ??= GetSession();

    protected abstract Task<IEntitySession> GetSession();
    protected abstract IEnumerable<IBrowseNameCollection> GetBrowseNameCollection();


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
        var strategy = TestedService;
        var session = await OpenedSession;
        var shouldFail = async () =>
            await strategy.TraverseUntil(session.Session, ObjectIds.RootFolder, e => false, token);
        await Assert.ThrowsAsync<EntityBrowseException>(shouldFail);
    }

    [Test]
    [InstanceMethodDataSource(nameof(PresentObjects))]
    public async Task WhenTraversingWithMatch_DoesNotThrowNotFound(NodeId id, CancellationToken token)
    {
        var session = await OpenedSession;
        var result = await TestedService
            .TraverseUntil(session.Session, ObjectIds.RootFolder, e => e.NodeId.Equals(id), token);

        await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task WhenTraversing_DoesNotGiveDuplicateNodes(CancellationToken token)
    {
        var strategy = TestedService;
        var session = await OpenedSession;

        var duplicates = await strategy
            .TraverseFrom(ObjectIds.RootFolder, session.Session, token)
            .Collect()
            .SelectAsync(nodeReferences => nodeReferences
                .Select<ReferenceWithId, NodeId>(n => n.NodeId)
                .GroupBy(x => x).Where(g => g.Count() > 1)
                .ToList()
            );

        foreach (var duplicate in duplicates)
        {
            await Assert.That(duplicate.Count()).IsLessThan(1).Because(duplicate.Key +
                                                                       " should only be returned once but was returned " +
                                                                       duplicates.Count + " times");
        }
    }

    [Test]
    [InstanceMethodDataSource(nameof(PresentObjects))]
    public async Task WhenLookingForSelf_DoesNotThrowNotFound(NodeId id, CancellationToken token)
    {
        var session = await OpenedSession;
        var result = await TestedService
            .TraverseUntil(session.Session, ObjectIds.RootFolder, e => e.NodeId.Equals(id), token);

        await Assert.That(result).IsNotNull();
    }

    [Test]
    [SuppressMessage("Maintainability", "S108", Justification = "The test must consume the traversal results")]
    public async Task WhenTraversingFromRoot_DoesNotLoopForever(CancellationToken ct)
    {
        var strategy = TestedService;
        var session = await OpenedSession;
        await foreach (var _ in strategy.TraverseFrom(ObjectIds.RootFolder, session.Session, ct))
        {
        }
    }

    [Test]
    [DisplayName("Can find entity root for all entities")]
    public async Task CanFindReferencesForAllNodes(CancellationToken token)
    {
        var session = await OpenedSession;
        var strategy = TestedService;

        foreach (var browseNameCollection in GetBrowseNameCollection())
        {
            await strategy.TraverseUntil(session.Session, ObjectIds.RootFolder,
                node => browseNameCollection.EntityName.Equals(node.BrowseName.Name),
                token);
        }
    }
}