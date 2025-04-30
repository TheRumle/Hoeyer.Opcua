using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public interface INodeTreeTraverser
{
    public IAsyncEnumerable<ReferenceWithId> TraverseFrom(NodeId id, ISession session, CancellationToken ct);

    public Task<ReferenceWithId> TraverseUntil(
        ISession session,
        NodeId root,
        Predicate<ReferenceDescription> predicate,
        CancellationToken token = default);
}