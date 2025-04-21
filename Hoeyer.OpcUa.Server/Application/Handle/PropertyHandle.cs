using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.Handle;

internal sealed record PropertyHandle : ManagedHandle<PropertyState>
{
    /// <inheritdoc />
    public PropertyHandle(PropertyState payload) : base(payload, DataTypes.DataValue)
    {
    }
}