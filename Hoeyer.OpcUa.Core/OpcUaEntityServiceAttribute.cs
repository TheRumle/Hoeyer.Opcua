﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core;

/// <summary>
/// A marker attribute marking generic classes as an Entity Service. An Entity service is a service that is instantiated with a type parameter T where T is annotated with the <seealso cref="OpcUaEntityAttribute"/>. The marker is used for reflection scanning. 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class OpcUaEntityServiceAttribute : Attribute
{
    public readonly bool IsGenericService;

    public readonly bool IsInterfaceService;

    public readonly Type ServiceType;

    public OpcUaEntityServiceAttribute(Type t, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        if (t.IsGenericTypeDefinition && t.GetGenericArguments().Length != 1)
        {
            throw new ArgumentException(
                $"When {t.Name} is a generic type definition it must take a single generic argument.");
        }

        Lifetime = lifetime;

        IsInterfaceService = t.IsInterface;
        IsGenericService = t.IsGenericTypeDefinition;
        ServiceType = t;
    }

    public ServiceLifetime Lifetime { get; }
}