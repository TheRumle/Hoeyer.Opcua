using System.Collections.Frozen;
using System.Reflection;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
using Hoeyer.OpcUa.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Configuration.Modelling;

public sealed class EntityTypeModel<TEntity> : IEntityTypeModel<TEntity>
{

    public EntityTypeModel(
        [FromKeyedServices(ServiceKeys.MODELLING)] ServiceAssemblyMarkerCollection markers,
        EntityTypesCollection collection) : this(markers, collection, typeof(TEntity))
    {
    }

    public EntityTypeModel(
        [FromKeyedServices(ServiceKeys.MODELLING)] ServiceAssemblyMarkerCollection markers,
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

        PropertyToBrowseName = entity.GetProperties().ToFrozenDictionary(
            property => property.Name,
            property => property.GetBrowseNameOrDefault(property.Name));

        Methods = BehaviourInterfaces.SelectMany(e => e.GetMethods()).ToFrozenSet();
        MethodNames = Methods.ToFrozenDictionary(e => e.Name, e => e.GetBrowseNameOrDefault(e.Name));

        PropertyAlarms = entity.GetProperties().ToFrozenDictionary(
            property => PropertyToBrowseName[property.Name],
            property => property.GetCustomAttributes<OpcAlarmAttribute>().Cast<IOpcAlarm>().ToList()
        );

        AlarmNames = PropertyAlarms
            .Values
            .SelectMany(e => e.Select(alarmAttr => alarmAttr.BrowseName))
            .ToFrozenSet();

        BrowseNameToEnumProperty = EntityType
            .GetProperties()
            .Where(prop => prop.PropertyType.IsEnum)
            .ToDictionary(e => PropertyToBrowseName[e.Name], e => e.PropertyType);
    }

    public IDictionary<string, Type> BrowseNameToEnumProperty { get; }


    public Type EntityType { get; }
    public FrozenSet<string> AlarmNames { get; set; }
    public IReadOnlyCollection<Type> BehaviourInterfaces { get; }
    public IReadOnlyCollection<MethodInfo> Methods { get; }

    /// <summary>
    /// Maps from property name to browse name
    /// </summary>
    public FrozenDictionary<string, string> MethodNames { get; set; }

    /// <summary>
    /// Maps from property name to browse name
    /// </summary>
    public FrozenDictionary<string, string> PropertyToBrowseName { get; set; }

    public FrozenDictionary<string, List<IOpcAlarm>> PropertyAlarms { get; set; }
    public string EntityName { get; }
    public override string ToString() => $"Entity model [{EntityName}]";
}