using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public class NodeBrowser : INodeBrowser
{
    public Task<BrowseResponse> BrowseById(ISession session, IEnumerable<NodeId> ids, NodeClass filter = NodeClassFilters.Any, CancellationToken ct = default)
    {
        var toBrowse = ids.Select(e => new BrowseDescription
        {
            BrowseDirection = BrowseDirection.Forward,
            NodeId = e,
            ResultMask = (uint)BrowseResultMask.All,
            NodeClassMask = (uint)filter,
        }).ToList();
        
        return session.BrowseAsync(
            null,
            null,
            250u,
            new BrowseDescriptionCollection(toBrowse),
            ct);
    }
}