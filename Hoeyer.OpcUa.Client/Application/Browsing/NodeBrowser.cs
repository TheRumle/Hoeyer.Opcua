using Opc.Ua;
using Opc.Ua.Client;
using INodeBrowser = Hoeyer.OpcUa.Client.Abstractions.Browsing.INodeBrowser;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public class NodeBrowser : INodeBrowser
{
    public ValueTask<BrowseResponse> BrowseById(ISession session, IEnumerable<NodeId> ids,
        NodeClass filter = NodeClass.Unspecified, CancellationToken ct = default) =>
        IdBrowse(session, ids, filter, ct);
    
    public ValueTask<BrowseResponse> BrowseById(ISession session, NodeId id, CancellationToken ct = default) =>
        BrowseById(session, [id], filter: NodeClass.Unspecified, ct: ct);

    private static ValueTask<BrowseResponse> IdBrowse(ISession session, IEnumerable<NodeId> ids, NodeClass filter, CancellationToken ct)
    {
        List<BrowseDescription> toBrowse = ids.Select(e => new BrowseDescription
        {
            BrowseDirection = BrowseDirection.Forward,
            NodeId = e,
            ResultMask = (uint)BrowseResultMask.All,
            NodeClassMask = (uint)filter
        }).ToList();

        return session.BrowseAsync(
            null,
            null,
            250u,
            toBrowse,
            ct);
    }

    
        
}