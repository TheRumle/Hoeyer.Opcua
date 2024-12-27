using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity;

internal interface IEntityObjectStateCreator
{
    public BaseObjectState CreateEntityOpcUaNode(ushort namespaceIndex);
}