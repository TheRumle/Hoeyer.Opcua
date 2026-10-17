using System.Diagnostics.CodeAnalysis;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

[SuppressMessage("Maintainability", checkId: "S2326", Justification = "Used for injection and modelling")]
public interface IEntityNodeMethodAssigner<T> : IEntityNodeMethodAssigner;

public interface IEntityNodeMethodAssigner
{
    IEnumerable<MethodState> AssignMethods(BaseObjectState entity);
}