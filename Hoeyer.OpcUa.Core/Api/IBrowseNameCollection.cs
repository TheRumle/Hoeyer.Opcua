using System.Collections.Frozen;

namespace Hoeyer.OpcUa.Core.Api;

public interface IBrowseNameCollection<T> : IBrowseNameCollection;

public interface IBrowseNameCollection
{
    FrozenDictionary<string, string> MethodNames { get; }
    FrozenDictionary<string, string> PropertyNames { get; }
    string EntityName { get; }
}