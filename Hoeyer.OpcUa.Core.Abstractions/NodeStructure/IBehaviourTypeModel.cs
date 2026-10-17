using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IBehaviourTypeModel
{
    IReadOnlyCollection<Type> BehaviourInterfaces { get; }
    IReadOnlyCollection<MethodInfo> Methods { get; }
}

[SuppressMessage("Maintainability", checkId: "S2326", Justification = "Used for injection and modelling")]
public interface IBehaviourTypeModel<T> : IBehaviourTypeModel;