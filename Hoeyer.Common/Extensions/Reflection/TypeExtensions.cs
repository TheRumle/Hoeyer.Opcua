using System;
using System.Linq;
using System.Reflection;

namespace Hoeyer.Common.Extensions.Reflection;

public static class TypeExtensions
{
    public static bool IsAnnotatedWith<T>(this Type type) where T : Attribute
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (type.GetCustomAttribute<T>() != null || type.GetInterfaces().Any(i => i.GetCustomAttribute<T>() != null))
            return true;

        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.GetCustomAttribute<T>() != null)
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }
    
}