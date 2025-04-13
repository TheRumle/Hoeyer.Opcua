using System;

namespace Hoeyer.OpcUa.Core;

/// <summary>
/// A marker attribute marking generic classes as an Entity Service. An entity service is a service that is instantiated with a type parameter T where T is annotated with the <seealso cref="OpcUaEntityAttribute"/>. The marker is used for reflection scanning. 
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class OpcUaEntityServiceAttribute : Attribute
{
    public OpcUaEntityServiceAttribute(Type t)
    {
        if (t.IsGenericTypeDefinition && t.GetGenericArguments().Length != 1)
        {
            throw new ArgumentException($"{t.Name} must be a generic type taking a single parameter.");
        }

        IsInterfaceService = t.IsInterface;
        IsGenericService = t.IsGenericTypeDefinition;
        ServiceType = t;
    }

    public readonly bool IsInterfaceService;

    public readonly bool IsGenericService;

    public readonly Type ServiceType;
}