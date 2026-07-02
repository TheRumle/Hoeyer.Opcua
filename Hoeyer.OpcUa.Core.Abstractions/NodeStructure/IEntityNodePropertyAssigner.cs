using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IEntityNodePropertyAssigner<T>
{
    IEnumerable<PropertyState> AssignProperties(BaseObjectState entity);
}