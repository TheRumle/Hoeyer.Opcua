using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hoeyer.Common.Reflection;

public static class TypeExtensions
{
    public static ICollection<Assembly> GetConsumingAssemblies(this Type marker)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a =>
            {
                try
                {
                    return a.GetReferencedAssemblies()
                        .Any(r => r.FullName == marker.Assembly.FullName);
                }
                catch
                {
                    // Some dynamic assemblies might throw
                    return false;
                }
            })
            .ToList();
    }
    
    public static bool IsAnnotatedWith<T>(this Type type) where T : Attribute
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (type.GetCustomAttribute<T>() != null || type.GetInterfaces().Any(i => i.GetCustomAttribute<T>() != null))
        {
            return true;
        }

        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.GetCustomAttribute<T>() != null)
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }
}