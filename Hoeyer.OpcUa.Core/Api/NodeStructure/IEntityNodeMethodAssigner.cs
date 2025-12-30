using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api.NodeStructure;

public interface IEntityNodeMethodAssigner<T>
{
    IEnumerable<MethodState> AssignMethods(BaseObjectState entity);
}