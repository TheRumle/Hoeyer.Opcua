using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Reading;

public interface INodeReader
{
    public Task<IEnumerable<ReadResult>> ReadNodesAsync(
        ISession session,
        IEnumerable<NodeId> ids,
        NodeClass filter = NodeClassFilters.Any,
        CancellationToken ct = default);
}