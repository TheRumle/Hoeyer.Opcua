using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Enum)]
public sealed class OpcUaAlarmAttribute<T> : Attribute
{
    public readonly Type Entity = typeof(T);
}