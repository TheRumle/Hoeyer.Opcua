using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Application.Handle;

internal sealed record PropertyHandle : ManagedHandle<PropertyState>
{
    /// <inheritdoc />
    public PropertyHandle(PropertyState payload) : base(payload, DataTypes.DataValue)
    {
    }
}