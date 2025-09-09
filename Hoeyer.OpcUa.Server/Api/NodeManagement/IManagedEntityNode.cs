using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

/// <summary>
/// A wrapper around an <see cref="IEntityNode"/> ensuring exposing state-change methods that can be used to securely operate on the node. For changing the state of the node see <see cref="ILocked{T}"/>.
/// </summary>
public interface IManagedEntityNode : ILocked<IEntityNode>
{
    string Namespace { get; }
    ushort EntityNameSpaceIndex { get; }

    string EntityName { get; }
}

public interface IManagedEntityNode<out T> : IManagedEntityNode;