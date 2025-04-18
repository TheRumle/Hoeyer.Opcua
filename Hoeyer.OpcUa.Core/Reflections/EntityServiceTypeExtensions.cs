﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Hoeyer.OpcUa.Core.Reflections;

public static class EntityServiceTypeExtensions
{
    public static IEnumerable<EntityServiceTypeContext> GetEntityServicesOfType(this IEnumerable<Type> assemblyTypes,
        Type service)
    {
        if (!service.IsGenericTypeDefinition || service.GetGenericArguments().Length > 1)
        {
            throw new OpcUaEntityServiceConfigurationException(
                "The specified type does not represent a Generic Type Definition. The service type must take 1 generic argument, must be an interface, and must not be a type representing an instantiation of the generic type definition.");
        }

        foreach (var type in assemblyTypes.Where(t => t.IsClass && !t.IsAbstract))
            if (EntityServiceTypeContext.TryCreateFromTypeImplementing(type, service, out var context))
            {
                yield return context;
            }
    }
}