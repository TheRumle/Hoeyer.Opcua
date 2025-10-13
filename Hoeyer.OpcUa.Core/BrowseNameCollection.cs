using System;
using System.Collections.Frozen;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructureFactory;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;

namespace Hoeyer.OpcUa.Core;

public class BrowseNameCollection : IBrowseNameCollection
{
    public BrowseNameCollection(Type type)
    {
        EntityName = type.GetBrowseNameOrDefault(type.Name);
        PropertyNames = type.GetProperties().ToFrozenDictionary(
            property => property.Name,
            property => property.GetBrowseNameOrDefault(property.Name));

        MethodNames = OpcUaEntityTypes.MethodsByEntity[type]
            .ToFrozenDictionary(e => e.Name,
                e => e.GetBrowseNameOrDefault(e.Name));
    }

    public FrozenDictionary<string, string> MethodNames { get; set; }

    public FrozenDictionary<string, string> PropertyNames { get; set; }

    public string EntityName { get; set; }

    /// <inheritdoc />
    public override string ToString() => "Browse names for " + EntityName;
}

public sealed class BrowseNameCollection<T>() : BrowseNameCollection(typeof(T)), IBrowseNameCollection<T>;