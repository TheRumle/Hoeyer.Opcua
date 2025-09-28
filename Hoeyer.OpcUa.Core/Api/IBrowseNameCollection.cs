using System.Collections.Frozen;

namespace Hoeyer.OpcUa.Core.Api;

public interface IBrowseNameCollection<T> : IBrowseNameCollection;

public interface IBrowseNameCollection
{
    FrozenDictionary<string, string> MethodNames { get; set; }
    FrozenDictionary<string, string> PropertyNames { get; set; }
    string EntityName { get; set; }
}