using System.Collections.Frozen;

namespace Hoeyer.OpcUa.Core.Abstractions;

public interface IBrowseNameCollection<T> : IBrowseNameCollection
{
    public new Type EntityType => typeof(T);
}

public interface IBrowseNameCollection
{
    FrozenDictionary<string, string> MethodNames { get; }
    FrozenDictionary<string, string> PropertyToBrowseName { get; }
    public IDictionary<string, Type> BrowseNameToEnumProperty { get; }
    string EntityName { get; }
    public Type EntityType { get; }
}