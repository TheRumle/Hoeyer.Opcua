using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Configuration.Entities;

internal record PossiblePropertyDataMatch(PropertyConfiguration Property, DataValue DataValue)
{
    public DataValue DataValue { get; } = DataValue;
    public PropertyConfiguration Property { get; } = Property;
}