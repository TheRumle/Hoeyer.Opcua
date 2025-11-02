using System.Collections.Frozen;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Configuration.Modelling;

public sealed class EntityTypesCollection
{
    public EntityTypesCollection([FromKeyedServices(ServiceKeys.MODELLING)] IEnumerable<AssemblyMarker> markers)
    {
        var markerTypes = markers.SelectMany(e => e.TypesInAssembly).ToList();
        ModelledEntities = markerTypes
            .Where(type => type.IsAnnotatedWith<OpcUaEntityAttribute>())
            .ToFrozenSet();
    }

    public FrozenSet<Type> ModelledEntities { get; }
    public bool ContainsEntity(Type entity) => ModelledEntities.Contains(entity);
}