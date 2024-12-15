using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration;

public readonly record struct PossiblePropertyMatch(
    IEnumerable<(PropertyConfiguration property, DataValue dataValue)> Matches)
{
    public IEnumerable<(PropertyConfiguration property, DataValue dataValue)> Matches { get; } = Matches;
}