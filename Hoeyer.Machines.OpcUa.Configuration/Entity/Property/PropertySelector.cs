using System;
using System.Linq.Expressions;
using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.Entity.Context;
using Hoeyer.Machines.OpcUa.Configuration.Entity.Exceptions;

namespace Hoeyer.Machines.OpcUa.Configuration.Entity.Property;

public class PropertySelector<TNodeType>
{
    private OpcUaEntityConfiguration _context;
    internal PropertySelector(OpcUaEntityConfiguration _context)
    {
        this._context = _context;
    }
    public PropertyConfigurator Property<TProperty>(
        Expression<Func<TNodeType, TProperty>> expression)
    {
        PropertyInfo property = GetPropertyInfo(expression);
        VerifyOrThrow(property);
        return new PropertyConfigurator(_context, property);
    }

    private static PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TNodeType, TProperty>> expression)
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