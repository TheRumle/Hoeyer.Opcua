using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api.NodeStructure;

public sealed record AlarmCollection
{
    internal IList<LimitAlarmState> PropertyAlarms { get; } = new List<LimitAlarmState>();
}