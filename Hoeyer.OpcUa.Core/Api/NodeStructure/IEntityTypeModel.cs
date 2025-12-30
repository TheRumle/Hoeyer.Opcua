using System.Collections.Frozen;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Api.NodeStructure;

public interface IEntityTypeModel<T> : IBehaviourTypeModel<T>, IBrowseNameCollection<T>
{
    public FrozenDictionary<string, List<OpcAlarmAttribute>> PropertyAlarms { get; }
}