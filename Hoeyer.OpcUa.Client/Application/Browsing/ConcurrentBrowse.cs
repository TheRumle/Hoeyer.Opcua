using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Opc.Ua;
using Opc.Ua.Client;
using INodeBrowser = Hoeyer.OpcUa.Client.Api.Browsing.INodeBrowser;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

internal sealed class ConcurrentBrowse(INodeBrowser browser, IProducerConsumerCollection<ReferenceWithId> queue)
{
    private readonly ConcurrentDictionary<NodeId, ReferenceWithId> _visited = new();

    public async IAsyncEnumerable<ReferenceWithId> Browse(
        ISession session,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        queue.TryTake(out ReferenceWithId root);
        _visited.TryAdd(root.NodeId, root);
        queue.TryAdd(root);
        while (!cancellationToken.IsCancellationRequested)
        {
            if (cancellationToken.IsCancellationRequested) break;
            List<ReferenceWithId> visiting = DequeueBatchOfSize(30).ToList();
            if (visiting.Count == 0) break;
            List<ReferenceWithId> neighbours = await FindNeighbours(session, cancellationToken, visiting);

            foreach (ReferenceWithId? reference in neighbours)
            {
                queue.TryAdd(reference);
            }

            foreach (ReferenceWithId? visited in visiting)
            {
                yield return visited;
            }
        }
    }


    private async Task<List<ReferenceWithId>> FindNeighbours(ISession session, CancellationToken cancellationToken,
        List<ReferenceWithId> visiting)
    {
        return await browser.BrowseById(session, visiting.Select(e => e.NodeId), ct: cancellationToken)
            .ThenAsync(browseResults => browseResults
                .Results
                .SelectMany<BrowseResult, ReferenceDescription>(browseResult => browseResult.References)
                .Select(reference => new ReferenceWithId(session, reference))
                .Where(e => _visited.TryAdd(e.NodeId, e))
                .ToList());
    }

    private IEnumerable<ReferenceWithId> DequeueBatchOfSize(int size)
    {
        var i = size;
        while (i > 0 && queue.TryTake(out var res))
        {
            yield return res;
            i--;
        }
    }
}