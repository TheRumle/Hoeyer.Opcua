using System;
using System.Collections.Frozen;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructureFactory;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;

namespace Hoeyer.OpcUa.Core;

public sealed class BrowseNameCollection<T> : IBrowseNameCollection<T>
{
    private readonly Type _type = typeof(T);

    public BrowseNameCollection()
    {
        EntityName = _type.GetBrowseNameOrDefault(_type.Name);
        PropertyNames = _type.GetProperties().ToFrozenDictionary(
            property => property.Name,
            property => property.GetBrowseNameOrDefault(property.Name));

        MethodNames = OpcUaEntityTypes.MethodsByEntity[_type]
            .ToFrozenDictionary(e => e.Name,
                e => e.GetBrowseNameOrDefault(e.Name));
    }

    public FrozenDictionary<string, string> MethodNames { get; set; }

    public FrozenDictionary<string, string> PropertyNames { get; set; }

    public string EntityName { get; set; }
}