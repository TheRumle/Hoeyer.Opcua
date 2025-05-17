using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hoeyer.Common.Reflection;

public static class AssemblyExtensions
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
                    return false;
                }
            })
            .ToList();
    }

    public static IEnumerable<Type> GetTypesFromConsumingAssemblies(this Type marker)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try
                {
                    if (a.GetReferencedAssemblies()
                        .Any(r => r.FullName == marker.Assembly.FullName))
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

    public static Type? GetAnnotationInstance(this Type type, Type annotation)
    {
        if (!typeof(Attribute).IsAssignableFrom(annotation))
            throw new ArgumentException(annotation.FullName + " is not an Attribute");
        if (type == null) throw new ArgumentNullException(nameof(type));

        foreach (Type attributeType in type.GetCustomAttributes().Select(e => e.GetType()))
        {
            if (attributeType.IsAssignableFrom(annotation)) return attributeType;

            if (!attributeType.IsConstructedGenericType ||
                annotation.GenericTypeArguments.Length != attributeType.GetGenericArguments().Length)
                continue;

            if (IsSameGenericAttribute(annotation, attributeType)) return attributeType;
        }

        return null;
    }

    private static bool IsSameGenericAttribute(Type annotation, Type attributeType) =>
        attributeType == annotation || attributeType.IsAssignableFrom(annotation) ||
        (attributeType.IsConstructedGenericType
         && annotation.IsGenericTypeDefinition
         && attributeType.GetGenericTypeDefinition() == annotation);
}