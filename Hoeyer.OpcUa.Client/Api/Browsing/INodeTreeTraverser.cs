using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public interface INodeTreeTraverser
{
    public IAsyncEnumerable<ReferenceDescription> TraverseFrom(ISession session, NodeId id,  CancellationToken ct);

    public Task<ReferenceDescription> TraverseUntil(
        ISession session,
        NodeId root,
        Predicate<ReferenceDescription> predicate,
        CancellationToken token = default);
}