using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Exceptions;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Opc.Ua;
using Opc.Ua.Client;
using INodeBrowser = Hoeyer.OpcUa.Client.Api.Browsing.INodeBrowser;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public abstract class ConcurrentTreeTraversalStrategy(
    INodeBrowser browser,
    INodeReader reader,
    Func<IProducerConsumerCollection<ReferenceWithId>> orderedCollectionFactory) : INodeTreeTraverser
{
    public async IAsyncEnumerable<ReferenceWithId> TraverseFrom(NodeId id, ISession session,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var _queue = orderedCollectionFactory.Invoke();
        var node = await reader.ReadNodeAsync(session, id, ct);
        if (node == null) throw new InvalidBrowseRootException(id);
        var rootRef = new ReferenceWithId(id, new ReferenceDescription
        {
            NodeId = id,
            NodeClass = node.NodeClass,
            BrowseName = node.BrowseName,
            DisplayName = node.DisplayName,
            TypeDefinition = node.TypeDefinitionId,
        });
        _queue.TryAdd(rootRef);
        yield return rootRef;


        await foreach (var a in new ConcurrentBrowse(browser, _queue).Browse(session, ct))
            yield return a;
    }

    public async Task<ReferenceWithId> TraverseUntil(
        ISession session,
        NodeId root,
        Predicate<ReferenceDescription> predicate,
        CancellationToken token = default)
    {
        using var whenFoundCancel = new CancellationTokenSource();
        using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, whenFoundCancel.Token);
        await foreach (var referenceDescription in TraverseFrom(root, session, combinedTokenSource.Token))
        {
            if (!predicate(referenceDescription.Description))
            {
                continue;
            }

            if (!combinedTokenSource.IsCancellationRequested)
            {
                combinedTokenSource.Cancel();
            }

            return referenceDescription;
        }

        throw new EntityBrowseException("No reference matching the predicate was found");
    }
}