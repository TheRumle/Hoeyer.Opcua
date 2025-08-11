using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class OpcUaEntityMethodsAttribute<T> : Attribute
{
    public Type EntityTarget { get; } = typeof(T);
}

