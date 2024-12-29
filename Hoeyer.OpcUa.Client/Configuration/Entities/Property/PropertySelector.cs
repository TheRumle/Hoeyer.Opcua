using System;
using System.Linq.Expressions;
using System.Reflection;
using Hoeyer.OpcUa.Client.Configuration.Entities.Exceptions;

namespace Hoeyer.OpcUa.Client.Configuration.Entities.Property;

public class PropertySelector<TEntity>
{
    private readonly EntityConfiguration<TEntity> _context;
    internal PropertySelector(EntityConfiguration<TEntity> _context)
    {
        this._context = _context;
    }
    public PropertyConfigurator<TEntity> Property<TProperty>(
        Expression<Func<TEntity, TProperty>> expression)
    {
        PropertyInfo property = GetPropertyInfo(expression);
        VerifyOrThrow(property);
        return new PropertyConfigurator<TEntity>(_context, property);
    }

    private static PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TEntity, TProperty>> expression)
    {
        if (expression.Body is not MemberExpression { Member: PropertyInfo propertyInfo })
            throw new InvalidPropertyConfigurationException();
        
        return propertyInfo;

    }

    private static void VerifyOrThrow(PropertyInfo propertyInfo)
    {
        var type = propertyInfo.PropertyType;
        if (!type.IsPrimitive && !type.IsEnum && type != typeof(string)) 
            throw new IncompatibleTypesException(propertyInfo);
    }
}