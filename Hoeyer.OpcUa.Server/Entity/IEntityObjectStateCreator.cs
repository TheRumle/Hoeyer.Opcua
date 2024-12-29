using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity;

public interface IEntityObjectStateCreator
{
    public BaseObjectState CreateEntityOpcUaNode(ISystemContext context, NodeState root, ushort namespaceIndex);
}