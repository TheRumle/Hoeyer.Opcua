using System;
using System.Linq;
using System.Reflection;

namespace Hoeyer.OpcUa.Simulation.Api;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
public sealed class OpcMethodArgumentsAttribute<TAgent, TInterface> : Attribute, IOpcMethodArgumentsAttribute
{
    public OpcMethodArgumentsAttribute(string methodName)
    {
        Agent = typeof(TAgent);
        Interface = typeof(TInterface);
        MethodName = methodName;
        Method = Interface.GetMember(methodName)
            .OfType<MethodInfo>()
            .First();
    }

    public MethodInfo Method { get; }

    public Type Agent { get; }
    public Type Interface { get; }
    public string MethodName { get; }
}