using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Configuration.Entities;

internal record PossiblePropertyDataMatch(PropertyConfiguration PropertyConfiguration, DataValue DataValue)
{
    public DataValue DataValue { get; } = DataValue;
    public PropertyConfiguration PropertyConfiguration { get; } = PropertyConfiguration;
}