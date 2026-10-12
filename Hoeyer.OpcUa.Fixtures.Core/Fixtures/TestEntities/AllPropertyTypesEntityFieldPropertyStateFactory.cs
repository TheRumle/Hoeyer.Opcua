using System.Reflection;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
using Hoeyer.OpcUa.Core.Application.NodeStructure;
using Hoeyer.OpcUa.Core.Application.OpcTypeMappers;
using Opc.Ua;

namespace Hoeyer.OpcUa.Fixtures.Core.Fixtures.TestEntities;

public class AllPropertyTypesEntityFieldPropertyStateFactory(
    //PropName
    IEntityTypeModel<AllPropertyTypesEntity> model
) : FieldPropertyStateFactory, IPropertyStateFactory<AllPropertyTypesEntity>
{
    private readonly Dictionary<string, Type> BrowseNameToType = model
        .EntityType
        .GetProperties()
        .Where(prop => prop.PropertyType.IsEnum)
        .ToDictionary(e => model.PropertyToBrowseName[e.Name], e => e.PropertyType)
        .ToDictionary();

    protected override PropertyState HandleEnumeration(
        BaseInstanceState parent,
        string browseName,
        PropertyInfo propertyInfo
    )
    {
        if (model.BrowseNameToEnumProperty.TryGetValue(browseName, out var type))
        {
            if (type == typeof(AllPropertyTypesEntity.EnumValueProp))
            {
                //PropName
                return PropertyState<AllPropertyTypesEntity.EnumValueProp>
                    .With<EnumVariantConverter<AllPropertyTypesEntity.EnumValueProp>>(parent);
            }
        }

        throw new InvalidOperationException("There was enum property with browsename " + browseName);
    }
}