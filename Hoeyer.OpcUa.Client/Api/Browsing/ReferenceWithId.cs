using System;
using Hoeyer.OpcUa.Core.Extensions;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public sealed class ReferenceWithId(NodeId nodeId, ReferenceDescription description) : IEquatable<ReferenceWithId>
{
    public readonly ReferenceDescription Description = description;
    public readonly NodeId NodeId = nodeId;

    public ReferenceWithId(ISession session, ReferenceDescription description)
        : this(description.NodeId.AsNodeId(session.NamespaceUris), description)
    {
    }

    public bool Equals(ReferenceWithId other) => NodeId.ToString()
        .Equals(other.NodeId.ToString());

    public override bool Equals(object? obj)
    {
        return obj is ReferenceWithId other && Equals(other);
    }

    public override int GetHashCode() => NodeId.GetHashCode();
}