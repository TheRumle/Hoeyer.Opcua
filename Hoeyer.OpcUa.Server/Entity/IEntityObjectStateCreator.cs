using System.Diagnostics.Contracts;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity;

public interface IEntityObjectStateCreator
{
    public string EntityName { get; }
    
    [Pure]
    public BaseObjectState CreateEntityOpcUaNode(ISystemContext context, NodeState root, ushort namespaceIndex);
}