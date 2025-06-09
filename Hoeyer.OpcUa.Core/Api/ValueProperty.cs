using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api;

public sealed class ValueProperty(PropertyState state)
{
    public string Name { get; } = state.BrowseName.Name;
    public NodeId NodeId { get; } = state.NodeId;
    public object Value { get; set; } = state.WrappedValue.Value;
    public object Handle { get; } = state.Handle;
}