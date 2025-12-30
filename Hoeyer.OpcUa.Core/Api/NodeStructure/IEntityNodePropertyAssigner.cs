using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api.NodeStructure;

public interface IEntityNodePropertyAssigner<T>
{
    IEnumerable<PropertyState> AssignProperties(BaseObjectState entity);
}