using System;
using System.Linq;

namespace Hoeyer.Machines.OpcUa.Configuration.Reflection.Types;

public static class ConcreteTypeReflections
{
    /// <summary>
    /// Checks if the type is a generic version of the @interface type. 
    /// </summary>
    /// <param name="type">A type representing a generic type, for instance typeof(List<>)</param>
    /// <param name="interface">A type representing the non-generic version of the type, for instance typeof(List<>)</param>
    /// <returns>True if type is a generic version of @interface</returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool ImplementsGenericInterfaceOf(this Type type, Type @interface)
    {
        if (!@interface.IsGenericTypeDefinition)
            throw new ArgumentException("The parameter 'interface' must be a generic type definition.");

        return GetGenericInterfaceDefinition(type, @interface) != null;
    }

    public static Type? GetGenericInterfaceDefinition(this Type type, Type @interface)
    {
        if (!@interface.IsGenericTypeDefinition)
            throw new ArgumentException("The parameter 'interface' must be a generic type definition.");

        return type is { IsClass: true, IsAbstract: false, IsPublic: true } 
            ? Array.Find(type.GetInterfaces(),inteface =>inteface.IsGenericType 
                                                         && inteface.GetGenericTypeDefinition() == @interface) 
            : null;
    }

    
    public static bool HasGenericParameterOfType(this Type type, Type parameter)
    {
        return type.IsGenericType && type.GetGenericArguments().Contains(parameter);
    }
}