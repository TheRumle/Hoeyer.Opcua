using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Common.Extensions.Types;

namespace Hoeyer.Common.Reflection;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetTypesFromConsumingAssemblies(this Type marker)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try
                {
                    var referenced = a.GetReferencedAssemblies();
                    if (referenced is not null && referenced.Any(r => r.FullName == marker.Assembly.FullName))
                    {
                        return a.GetTypes();
                    }

                    return [];
                }
                catch
                {
                    return [];
                }
            })
            .Union(marker.Assembly.GetTypes())
            .ToList();
    }

    public static Attribute? GetInstantiatedGenericAttribute(this Type type, Type genericAttribute)
    {
        return type.GetCustomAttributes().FirstOrDefault(e => e.GetType().IsClosedGenericOf(genericAttribute));
    }

    public static bool IsAnnotatedWith<T>(this Type type) where T : Attribute
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (type.GetCustomAttributes<T>().Any() || type.GetInterfaces().Any(i => i.GetCustomAttribute<T>() != null))
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