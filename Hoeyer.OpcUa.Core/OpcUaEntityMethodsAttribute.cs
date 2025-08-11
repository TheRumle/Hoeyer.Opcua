using System;

namespace Hoeyer.OpcUa.Core;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class OpcUaAgentMethodsAttribute<T> : Attribute
{
    public Type AgentTarget { get; } = typeof(T);
}

