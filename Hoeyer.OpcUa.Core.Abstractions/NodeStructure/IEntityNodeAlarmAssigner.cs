using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IEntityNodeAlarmAssigner<T>
{
    public AlarmCollection AssignAlarms(IEnumerable<PropertyState> properties, ushort applicationNamespaceIndex);
}