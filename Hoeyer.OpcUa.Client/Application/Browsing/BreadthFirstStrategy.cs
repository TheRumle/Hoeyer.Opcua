using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Core.Extensions;
using Opc.Ua;
using Opc.Ua.Client;
using INodeBrowser = Hoeyer.OpcUa.Client.Api.Browsing.INodeBrowser;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public sealed class BreadthFirstStrategy(INodeBrowser browser) : IDisposable, INodeTreeTraverser
{
    private readonly Queue<NodeId> _queue = new();
    private readonly ConcurrentDictionary<string, byte> _visited = new();
    public IAsyncEnumerable<ReferenceDescription> TraverseFrom(ISession session, NodeId id, CancellationToken ct)
    {
        _visited[id.ToString()] = 0;
        _queue.Enqueue(id);
        return TraverseQueue(session, ct);
    }
    
    public async Task<ReferenceDescription> TraverseUntil(
        ISession session,
        NodeId root,
        Predicate<ReferenceDescription> predicate,
        CancellationToken token = default)
    {
        _queue.Enqueue(root);
        using var whenFoundCancel = new CancellationTokenSource();
        using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, whenFoundCancel.Token);
        await foreach (var referenceDescription in TraverseQueue(session, combinedTokenSource.Token))
        {
            if (!predicate(referenceDescription))
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

    private async IAsyncEnumerable<ReferenceDescription> TraverseQueue(
        ISession session,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (_queue.Count > 0 && !cancellationToken.IsCancellationRequested)
        {
            var visiting = DequeueBatchOfSize(20).ToList();
            var browseResult = await browser.BrowseById(session, visiting, ct: cancellationToken);
            if (browseResult.Results.Count == 0) break;
            
            MarkVisited(visiting);
            if (cancellationToken.IsCancellationRequested) break;
            foreach (var p in EnqueueChildren(session, browseResult)) 
                yield return p;
        }
    }

    private IEnumerable<NodeId> DequeueBatchOfSize(int size)
    {
        return _queue.Where(e => !_visited.ContainsKey(e.ToString())).Take(size);
    }

    private void MarkVisited(IEnumerable<NodeId> nodesToBrowse)
    {
        foreach (var browsed in nodesToBrowse.Where(e=>e is not null))
        {
            _visited[browsed.ToString()] = 0;
        }
    }

    private IEnumerable<ReferenceDescription> EnqueueChildren(ISession session, BrowseResponse browseResults)
    {
        foreach (var reference in browseResults
                     .Results
                     .SelectMany(browseResult => browseResult.References))
        {
            _queue.Enqueue(reference.NodeId.AsNodeId(session.NamespaceUris));
            yield return reference;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _queue.Clear();
    }
}