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
        if (t is { IsInterface: true, IsGenericTypeDefinition: false } || t.GetGenericArguments().Length != 1)
        {
            throw new ArgumentException($"{t.Name} is not a singleparam generic interface! The type should represent the entity service the client service is offering.");
        }

        ServiceType = t;
    }

    public readonly Type ServiceType;
}