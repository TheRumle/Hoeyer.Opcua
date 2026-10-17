using System.Collections.Frozen;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IEntityTypeModel<T> : IEntityTypeModel, IBehaviourTypeModel<T>, IBrowseNameCollection<T>;

public interface IEntityTypeModel : IBehaviourTypeModel, IBrowseNameCollection
{
    public FrozenDictionary<string, List<IOpcAlarm>> PropertyAlarms { get; }
}