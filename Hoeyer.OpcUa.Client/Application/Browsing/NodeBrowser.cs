using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api;
using Opc.Ua;
using Opc.Ua.Client;
using INodeBrowser = Hoeyer.OpcUa.Client.Api.Browsing.INodeBrowser;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public class NodeBrowser : INodeBrowser
{
    public Task<BrowseResponse> BrowseById(ISession session, IEnumerable<NodeId> ids,
        NodeClass filter = NodeClassFilters.Any, CancellationToken ct = default)
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
            new BrowseDescriptionCollection(toBrowse),
            ct);
    }

    /// <inheritdoc />
    public Task<BrowseResponse> BrowseById(ISession session, NodeId id, CancellationToken ct = default) =>
        session.BrowseAsync(
            null,
            null,
            250u,
            new BrowseDescriptionCollection([
                new BrowseDescription
                {
                    BrowseDirection = BrowseDirection.Forward,
                    NodeId = id,
                    ResultMask = (uint)BrowseResultMask.All,
                    NodeClassMask = (uint)NodeClassFilters.Any
                }
            ]), ct);
}