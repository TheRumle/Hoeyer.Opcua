using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.Handle;

internal sealed record EntityHandle : ManagedHandle<BaseObjectState>
{
    public EntityHandle(BaseObjectState Value) : base(Value, DataTypes.ObjectNode)
    {
    }
}