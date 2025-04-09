using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.Types;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Reading;

internal sealed class NodeReader : INodeReader
{
    public async Task<IEnumerable<ReadResult>> ReadNodesAsync(
        ISession session,
        IEnumerable<NodeId> ids,
        NodeClass filter = NodeClassFilters.Any,
        CancellationToken ct = default)
    {
        return await session
            .ReadNodesAsync(ids.ToList(), filter, ct: ct)
            .ContinueWith(response =>
            {
                if (response.IsCompletedSuccessfully)
                    return response.Result.Zip().Select(result => new ReadResult(result.first, result.second));
                
                throw new NodeReadException(response.Exception!.Message);

            }, ct);
    }
    
}