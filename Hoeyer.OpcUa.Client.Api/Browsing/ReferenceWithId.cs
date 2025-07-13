using System;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public sealed class ReferenceWithId(NodeId nodeId, ReferenceDescription description) : IEquatable<ReferenceWithId>
{
    public readonly ReferenceDescription Description = description;
    public readonly NodeId NodeId = nodeId;

    public ReferenceWithId(ISession session, ReferenceDescription description)
        : this(ExpandedNodeId.ToNodeId(description.NodeId, session.NamespaceUris),
            description)
    {
    }

    public bool Equals(ReferenceWithId other) => NodeId.ToString()
        .Equals(other.NodeId.ToString());

    public override bool Equals(object? obj) => obj is ReferenceWithId other && Equals(other);

    public override int GetHashCode() => NodeId.GetHashCode();
}