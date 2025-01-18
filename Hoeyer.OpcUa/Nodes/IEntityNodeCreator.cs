using System.Diagnostics.Contracts;
using Opc.Ua;

namespace Hoeyer.OpcUa.Nodes;

public interface IEntityNodeCreator
{
    public string EntityName { get; }

    [Pure]
    public NodeState CreateEntityOpcUaNode(NodeState root, ushort namespaceIndex);
}