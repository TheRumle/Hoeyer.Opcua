using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Handle;

internal sealed record EntityHandle : ManagedHandle<BaseObjectState>
{
    public EntityHandle(BaseObjectState Value) : base(Value, DataTypes.ObjectNode)
    {
    }
}