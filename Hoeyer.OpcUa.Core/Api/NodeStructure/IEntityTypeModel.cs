using System;
using System.Collections.Frozen;
using System.Reflection;

namespace Hoeyer.OpcUa.Core.Api.NodeStructure;

public interface IBehaviourTypeModel
{
    FrozenSet<Type> BehaviourInterfaces { get; }
    FrozenSet<MethodInfo> Methods { get; }
}

public interface IEntityTypeModel<T> : IBehaviourTypeModel<T>, IBrowseNameCollection<T>
{
    FrozenSet<(string AlarmName, AlarmValue AlarmType)> Alarms { get; }
}