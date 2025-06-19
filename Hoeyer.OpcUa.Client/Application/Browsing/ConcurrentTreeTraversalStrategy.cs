using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public IAsyncEnumerable<ReferenceWithId> TraverseFrom(NodeId id, ISession session, CancellationToken ct)
    {
        IProducerConsumerCollection<ReferenceWithId>? queue = orderedCollectionFactory.Invoke();
        Node? node = reader.ReadNodeAsync(session, id, ct).Result;
        if (node == null)
        {
            throw new InvalidBrowseRootException(id);
        }

        var rootRef = new ReferenceWithId(id, new ReferenceDescription
        {
            NodeId = id,
            NodeClass = node.NodeClass,
            BrowseName = node.BrowseName,
            DisplayName = node.DisplayName,
            TypeDefinition = node.TypeDefinitionId
        });
        queue.TryAdd(rootRef);

        return new ConcurrentBrowse(browser, queue).Browse(session, ct);
    }

    public async Task<ReferenceWithId> TraverseUntil(
        ISession session,
        NodeId root,
        Predicate<ReferenceDescription> predicate,
        CancellationToken token = default)
    {
        using var whenFoundCancel = new CancellationTokenSource();
        using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, whenFoundCancel.Token);
        await foreach (ReferenceWithId? referenceDescription in TraverseFrom(root, session, combinedTokenSource.Token))
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