using System.Diagnostics.CodeAnalysis;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

[SuppressMessage("Maintainability", checkId: "S2326", Justification = "Used for injection and modelling")]
public interface IEntityNodePropertyAssigner<T> : IEntityNodePropertyAssigner
{
}

public interface IEntityNodePropertyAssigner
{
    IEnumerable<PropertyState> AssignProperties(BaseObjectState entity);
}