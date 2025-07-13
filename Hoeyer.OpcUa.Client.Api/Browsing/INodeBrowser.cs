using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public interface INodeBrowser
{
    Task<BrowseResponse> BrowseById(ISession session, IEnumerable<NodeId> ids,
        NodeClass filter = NodeClassFilters.Any,
        CancellationToken ct = default);

    Task<BrowseResponse> BrowseById(ISession session, NodeId id, CancellationToken ct = default);
}