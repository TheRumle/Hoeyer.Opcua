using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities;
internal record PossiblePropertyDataMatch( PropertyConfiguration Property, DataValue DataValue)
{
    public DataValue DataValue { get; } = DataValue;
    public  PropertyConfiguration Property { get; } = Property;
}