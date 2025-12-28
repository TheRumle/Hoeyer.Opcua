using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Reflection;

namespace Hoeyer.OpcUa.Core.Api.NodeStructure;

public interface IBehaviourTypeModel
{
    FrozenSet<Type> BehaviourInterfaces { get; }
    FrozenSet<MethodInfo> Methods { get; }
}

public interface IEntityTypeModel<T> : IBehaviourTypeModel<T>, IBrowseNameCollection<T>
{
    public FrozenDictionary<string, IEnumerable<OpcAlarmAttribute>> PropertyAlarms { get; set; }
}