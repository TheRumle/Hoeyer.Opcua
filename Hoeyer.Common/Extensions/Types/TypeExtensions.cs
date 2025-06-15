using System;
using System.Linq;

namespace Hoeyer.Common.Extensions.Types;

public static class TypeExtensions
{
    public static Type? GetImplementedVersionOfGeneric(this Type t, Type @interface)
    {
        return t.GetInterfaces().FirstOrDefault(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == @interface);
    }

    public static Type? Implements(this Type t, Type @interface)
    {
        return t.GetInterfaces().FirstOrDefault(i => i == @interface);
    }
}