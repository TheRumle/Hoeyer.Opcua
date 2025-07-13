using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hoeyer.Common.Extensions.Types;

public static class TypeExtensions
{
    public static Type? GetImplementedVersionOfGeneric(this Type t, Type @interface)
    {
        return t.GetInterfaces().FirstOrDefault(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == @interface);
    }

    public static IEnumerable<Type> GetAllImplementedVersionsOfGeneric(this Type t, Type @interface)
    {
        return t.GetInterfaces().Where(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == @interface);
    }

    public static Type? GetImplementationOf(this Type t, Type @interface)
    {
        return t.GetInterfaces().FirstOrDefault(i => i == @interface);
    }

    public static bool Implements(this Type t, Type @interface) => @interface.IsAssignableFrom(t);

    public static string GetFriendlyTypeName(this Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var sb = new StringBuilder();
        var name = type.Name;

        var index = name.IndexOf('`');
        if (index > 0)
        {
            name = name[..index];
        }

        sb.Append(name);
        sb.Append("<");

        Type[] args = type.GetGenericArguments();
        for (var i = 0; i < args.Length; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }

            sb.Append(GetFriendlyTypeName(args[i]));
        }

        sb.Append(">");
        return sb.ToString();
    }
}