using Opc.Ua;

namespace Hoeyer.Machines.OpcUa.Entities.Configuration;
internal record PossiblePropertyDataMatch( PropertyConfiguration Property, DataValue DataValue)
{
    public DataValue DataValue { get; } = DataValue;
    public  PropertyConfiguration Property { get; } = Property;
}