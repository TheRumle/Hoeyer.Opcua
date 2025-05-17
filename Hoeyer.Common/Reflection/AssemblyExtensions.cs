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
            //exactly the same type
            if (attributeType.IsAssignableFrom(annotation)) return attributeType;

            //input is generic definition - the attribute must be of that type
            if (annotation.IsGenericTypeDefinition
                && attributeType.IsGenericType && attributeType.GetGenericTypeDefinition() == annotation)
                return attributeType;

            //input is instantiated generic - all args match
            if (HasSameGenericArgs(annotation, attributeType)) return attributeType;
        }

        return null;
    }

    private static bool HasSameGenericArgs(Type annotation, Type attributeType)
    {
        if (attributeType.GenericTypeArguments.Length != annotation.GetGenericArguments().Length) return false;

        for (var index = 0; index < annotation.GenericTypeArguments.Length; index++)
        {
            Type first = attributeType.GenericTypeArguments[index];
            Type second = annotation.GenericTypeArguments[index];
            if (first != second) return false;
        }

        return true;
    }
}