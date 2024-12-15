using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Hoeyer.Machines.OpcUa.Reflection.Types;

internal static class InstanceFactory
{
    [Pure]
    public static (Type GenericInstantiation, TInstance Instance) CreateGenericInstance<TInstance>(this Type genericTypeDefinition,  Type[] genericParams,  object[] ctorArgs)
    {
        if(!genericTypeDefinition.IsGenericTypeDefinition)
            throw new ArgumentException("The type must be a generic type definition.");
        
        var genericType = genericTypeDefinition.MakeGenericType(genericParams);
        var instance = (TInstance) Activator.CreateInstance(
            genericType,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            null,
            ctorArgs,
            CultureInfo.InvariantCulture
        );
        return (GenericInstantiation: genericType, Instance: instance);
    }
    

  
    public static object CreateUninitalizedInstance(this Type typeDefinition)
    {
        return FormatterServices.GetUninitializedObject(typeDefinition);
    }
}