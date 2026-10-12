using System.Reflection;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IBehaviourTypeModel
{
    IReadOnlyCollection<Type> BehaviourInterfaces { get; }
    IReadOnlyCollection<MethodInfo> Methods { get; }
}

public interface IBehaviourTypeModel<T> : IBehaviourTypeModel;