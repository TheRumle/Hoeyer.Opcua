using System;
using System.Linq;
using System.Reflection;

namespace Hoeyer.Common.Reflection;

public static class AssemblyScanner
{
    public static Type[] FindTypesWithAttribute<T>() where T : Attribute
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t != null);
                }
            })
            .Where(t => t.IsDefined(typeof(T), false))
            .ToArray();
    }

    public static Type[] FindTypesWithGenericAttribute(Type genericAttribute)
    {
        if (genericAttribute == null)
        {
            throw new ArgumentNullException(nameof(genericAttribute));
        }

        if (!genericAttribute.IsGenericTypeDefinition ||
            !typeof(Attribute).IsAssignableFrom(genericAttribute))
        {
            throw new ArgumentException(
                "Type must be a generic attribute type definition",
                nameof(genericAttribute));
        }

        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t != null);
                }
            })
            .Where(t =>
                t.GetCustomAttributes(false)
                    .Any(attr =>
                        attr.GetType().IsGenericType &&
                        attr.GetType().GetGenericTypeDefinition() == genericAttribute))
            .ToArray();
    }
}