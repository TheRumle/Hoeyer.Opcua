using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IEntityNodeMethodAssigner<T>
{
    IEnumerable<MethodState> AssignMethods(BaseObjectState entity);
}