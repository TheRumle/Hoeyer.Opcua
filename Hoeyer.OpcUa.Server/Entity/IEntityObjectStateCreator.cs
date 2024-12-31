using System.Diagnostics.Contracts;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity;

public interface IEntityObjectStateCreator
{
    public string EntityName { get; }
    
    [Pure]
    public NodeState CreateEntityOpcUaNode(NodeState root, ushort namespaceIndex);
}