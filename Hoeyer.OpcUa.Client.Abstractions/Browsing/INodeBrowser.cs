using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Abstractions.Browsing;

public interface INodeBrowser
{
    ValueTask<BrowseResponse> BrowseById(ISession session, IEnumerable<NodeId> ids,
        NodeClass filter = NodeClass.Unspecified,
        CancellationToken ct = default);

    ValueTask<BrowseResponse> BrowseById(ISession session, NodeId id, CancellationToken ct = default);
}