using System;
using System.Linq;

namespace Hoeyer.Common.Reflection;

public static class TypeExtensions
{
    public static bool HasNonGenericAttribute<T>(this Type t, bool inherited = true)
    {
        return t.GetCustomAttributes(inherited).Any(attr => attr.GetType() == typeof(T));
    }

    public static bool HasGenericAttribute(this Type t, Type genericAttribute, bool inherited = true)
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

        return t.GetCustomAttributes(inherited)
            .Any(attr =>
                attr.GetType().IsGenericType &&
                attr.GetType().GetGenericTypeDefinition() == genericAttribute);
    }
}