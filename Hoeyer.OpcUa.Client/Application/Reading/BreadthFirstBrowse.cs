using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Hoeyer.OpcUa.Core.Extensions;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Reading;

internal sealed class BreadthFirstBrowse(Session session) : IDisposable
{
    private readonly Queue<NodeId> _queue = new();
    private readonly ConcurrentDictionary<string, byte> _visited = new();
    public IAsyncEnumerable<ReferenceDescription> TraverseFrom(NodeId id, CancellationToken ct)
    {
        _visited[id.ToString()] = 0;
        _queue.Enqueue(id);
        return TraverseQueue(ct);
    }
    
    public async Task<Result<ReferenceDescription>> SearchAsync(NodeId root, Expression<Predicate<ReferenceDescription>> predicate, CancellationToken token = default)
    {
        _queue.Enqueue(root);
        using var whenFoundCancel = new CancellationTokenSource();
        using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, whenFoundCancel.Token);
        var pred = predicate.Compile();
        await foreach (var referenceDescription in TraverseQueue(combinedTokenSource.Token))
        {
            if (!pred(referenceDescription))
            {
                continue;
            }

            if (!combinedTokenSource.IsCancellationRequested)
            {
                combinedTokenSource.Cancel();
            }
            return referenceDescription;
        }
        return Result.Fail($"No reference matching the predicate was found");
    }
    
    public IAsyncEnumerable<ReferenceDescription> TraverseFromRoot() => TraverseFrom(ObjectIds.RootFolder, CancellationToken.None);
    

    private async IAsyncEnumerable<ReferenceDescription> TraverseQueue([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (_queue.Count > 0 && !cancellationToken.IsCancellationRequested)
        {
            var nodesToBrowse = ConstructBrowseCollection().ToList();
            if (nodesToBrowse.Count == 0) break;
            
            var browseResults = await session.BrowseAsync(
                null,
                null,
                250u,
                new BrowseDescriptionCollection(nodesToBrowse),
                cancellationToken);
            
            foreach (var browsed in nodesToBrowse)
            {
                _visited[browsed.NodeId.ToString()] = 0;
            }

            if (cancellationToken.IsCancellationRequested) break;
            
            foreach (var reference in browseResults
                         .Results
                         .SelectMany(browseResult => browseResult.References))
            {
                _queue.Enqueue(reference.NodeId.ToNodeId(session.NamespaceUris));
                yield return reference;
            }
        }
    }
    private IEnumerable<BrowseDescription> ConstructBrowseCollection()
    {
        const uint classMask = (uint)NodeClass.Object | (uint)NodeClass.Variable | (uint)NodeClass.DataType |
                               (uint)NodeClass.View | (uint)NodeClass.ReferenceType;

        for (int i = 0; i < 10 && _queue.Count > 0; i++)
        {
            var nodeId = _queue.Dequeue();
            if (_visited.ContainsKey(nodeId.ToString())) continue;
            yield return new BrowseDescription
            {
                NodeId = nodeId,
                BrowseDirection = BrowseDirection.Forward,
                IncludeSubtypes = true,
                ResultMask = (uint)BrowseResultMask.All,
                NodeClassMask = classMask
            };
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _queue.Clear();
    }
}