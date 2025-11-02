using System;
using System.Collections.Frozen;
using System.Reflection;

namespace Hoeyer.OpcUa.Core.Api.NodeStructure;

public interface IEntityTypeModel : IBehaviourTypeModel, IBrowseNameCollection;

public interface IBehaviourTypeModel
{
    FrozenSet<Type> BehaviourInterfaces { get; }
    FrozenSet<MethodInfo> Methods { get; }
}

public interface IEntityTypeModel<T> : IEntityTypeModel, IBehaviourTypeModel<T>, IBrowseNameCollection<T>;