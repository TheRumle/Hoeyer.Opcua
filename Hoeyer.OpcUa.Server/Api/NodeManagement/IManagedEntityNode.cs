using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

/// <summary>
/// An entity node managed by an <see cref="IEntityNodeManager"/>
/// </summary>
public interface IManagedEntityNode : IEntityNode
{
    public object Lock { get; }
    string Namespace { get; }
    ushort EntityNameSpaceIndex { get; }
}

/// <inheritdoc />
public interface IManagedEntityNode<T> : IManagedEntityNode;