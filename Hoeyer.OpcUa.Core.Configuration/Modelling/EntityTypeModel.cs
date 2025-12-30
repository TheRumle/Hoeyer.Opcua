using System.Collections.Frozen;
using System.Reflection;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Hoeyer.OpcUa.Core.Application.NodeStructure;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Configuration.Modelling;

public sealed class EntityTypeModel<TEntity> : IEntityTypeModel<TEntity>
{
    public EntityTypeModel(Type marker) : this([new AssemblyMarker(marker)],
        new EntityTypesCollection([new AssemblyMarker(marker)]), typeof(TEntity))
    {
    }

    public EntityTypeModel(AssemblyMarker markers) : this([markers], new EntityTypesCollection([markers]),
        typeof(TEntity))
    {
    }

    public EntityTypeModel([FromKeyedServices(ServiceKeys.MODELLING)] IEnumerable<AssemblyMarker> markers,
        EntityTypesCollection collection) : this(markers, collection, typeof(TEntity))
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
            .Where(type => type is { IsGenericType: false, IsInterface: true })
            .Where(t => t.IsAnnotatedWith<OpcUaEntityMethodsAttribute<TEntity>>())
            .ToFrozenSet();


        EntityName = entity.GetBrowseNameOrDefault(entity.Name);

        PropertyNames = entity.GetProperties().ToFrozenDictionary(
            property => property.Name,
            property => property.GetBrowseNameOrDefault(property.Name));

        Methods = BehaviourInterfaces.SelectMany(e => e.GetMethods()).ToFrozenSet();
        MethodNames = Methods.ToFrozenDictionary(e => e.Name, e => e.GetBrowseNameOrDefault(e.Name));

        PropertyAlarms = entity.GetProperties().ToFrozenDictionary(
            property => PropertyNames[property.Name],
            property => property.GetCustomAttributes<OpcAlarmAttribute>().ToList()
        );

        AlarmNames = PropertyAlarms
            .Values
            .SelectMany(e => e.Select(alarmAttr => alarmAttr.BrowseName))
            .ToFrozenSet();
    }

    public Type EntityType { get; }
    public FrozenSet<string> AlarmNames { get; set; }
    public FrozenSet<Type> BehaviourInterfaces { get; }
    public FrozenSet<MethodInfo> Methods { get; }
    public FrozenDictionary<string, string> MethodNames { get; set; }
    public FrozenDictionary<string, string> PropertyNames { get; set; }
    public FrozenDictionary<string, List<OpcAlarmAttribute>> PropertyAlarms { get; set; }
    public string EntityName { get; }
    public override string ToString() => $"Entity model [{EntityName}]";
}