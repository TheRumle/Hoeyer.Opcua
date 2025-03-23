using System;
using System.Runtime.Serialization;

namespace Hoeyer.OpcUa.Core.Extensions.Types;

internal static class InstanceFactory
{
    public static object CreateUninitalizedInstance(this Type typeDefinition)
    {
        return FormatterServices.GetUninitializedObject(typeDefinition);
    }
}