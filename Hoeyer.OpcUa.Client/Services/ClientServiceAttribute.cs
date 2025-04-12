using System;

namespace Hoeyer.OpcUa.Client.Services;

[AttributeUsage(AttributeTargets.Class)]
internal class ClientServiceAttribute : Attribute
{
    public ClientServiceAttribute(Type t)
    {
        if (t is { IsInterface: true, IsGenericTypeDefinition: false } || t.GetGenericArguments().Length != 1)
        {
            throw new ArgumentException($"{t.Name} is not a singleparam generic interface! The type should represent the entity service the client service is offering.");
        }

        ServiceType = t;
    }

    public readonly Type ServiceType;
}