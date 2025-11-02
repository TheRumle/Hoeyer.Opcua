using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Hoeyer.OpcUa.Core.Application.NodeStructure;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Configuration.Modelling;

public sealed class EntityTypeModel<TEntity> : EntityTypeModel, IEntityTypeModel<TEntity>
{
    public EntityTypeModel(Type marker) : base([new AssemblyMarker(marker)],
        new EntityTypesCollection([new AssemblyMarker(marker)]), typeof(TEntity))
    {
    }

    public EntityTypeModel(AssemblyMarker markers) : base([markers], new EntityTypesCollection([markers]),
        typeof(TEntity))
    {
    }

    public EntityTypeModel([FromKeyedServices(ServiceKeys.MODELLING)] IEnumerable<AssemblyMarker> markers,
        EntityTypesCollection collection) : base(markers, collection, typeof(TEntity))
    {
    }
}

public class EntityTypeModel : IEntityTypeModel
{
    public EntityTypeModel(Type entity) : this([new AssemblyMarker(entity)],
        new EntityTypesCollection([new AssemblyMarker(entity)]), entity)
    {
    }

    public EntityTypeModel(Type marker, Type entity) : this([new AssemblyMarker(marker)],
        new EntityTypesCollection([new AssemblyMarker(marker)]), entity)
    {
    }

    public EntityTypeModel(AssemblyMarker markers, Type entity) : this([markers], new EntityTypesCollection([markers]),
        entity)
    {
    }

    public EntityTypeModel(
        [FromKeyedServices(ServiceKeys.MODELLING)]
        IEnumerable<AssemblyMarker> markers,
        EntityTypesCollection entityTypes,
        Type entity)
    {
        EntityType = entity;
        if (!entityTypes.ContainsEntity(entity))
        {
            throw new ModellingMismatchException(
                $"The type {entity.FullName} is not contained in the modelling landscape. Is the entity contained in the passed assembly markers?");
        }

        var markerTypes = markers
            .SelectMany(e => e.TypesInAssembly)
            .ToList();

        BehaviourInterfaces = markerTypes
            .ToImmutableHashSet()
            .Where(type => type is { IsGenericType: false, IsInterface: true })
            .Select(type => (
                type,
                isValid: type.GetInstantiatedGenericAttribute(typeof(OpcUaEntityMethodsAttribute<>)) != null
            ))
            .Where(e => e.isValid)
            .Select(e => e.type)
            .ToFrozenSet();

        Methods = BehaviourInterfaces.SelectMany(e => e.GetMethods()).ToFrozenSet();

        EntityName = entity.GetBrowseNameOrDefault(entity.Name);
        PropertyNames = entity.GetProperties().ToFrozenDictionary(
            property => property.Name,
            property => property.GetBrowseNameOrDefault(property.Name));

        MethodNames = Methods.ToFrozenDictionary(e => e.Name, e => e.GetBrowseNameOrDefault(e.Name));
    }

    public Type EntityType { get; }

    public FrozenSet<Type> BehaviourInterfaces { get; }
    public FrozenSet<MethodInfo> Methods { get; }
    public FrozenDictionary<string, string> MethodNames { get; set; }
    public FrozenDictionary<string, string> PropertyNames { get; set; }
    public string EntityName { get; }

    /// <inheritdoc />
    public override string ToString() => $"Entity model [{EntityName}]";
}