using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api.NodeStructure;

public interface IEntityNodeAlarmAssigner<T>
{
    public AlarmCollection AssignAlarms(IEnumerable<PropertyState> properties);
}