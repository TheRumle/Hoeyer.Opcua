using System.Collections.Frozen;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IEntityTypeModel<T> : IBehaviourTypeModel<T>, IBrowseNameCollection<T>
{
    public FrozenDictionary<string, List<IOpcAlarm>> PropertyAlarms { get; }
}