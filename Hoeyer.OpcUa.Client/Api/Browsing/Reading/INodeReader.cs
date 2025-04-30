using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Reading;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Browsing.Reading;

public interface INodeReader
{
    public Task<ReadResult> ReadNodesAsync(
        ISession session,
        IEnumerable<NodeId> ids,
        NodeClass filter = NodeClassFilters.Any,
        CancellationToken ct = default);
    
    public Task<Node> ReadNodeAsync(
        ISession session,
        NodeId nodeId,
        CancellationToken ct = default);
}