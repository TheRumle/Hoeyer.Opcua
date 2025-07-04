using System;
using System.Linq;
using System.Reflection;

namespace Hoeyer.OpcUa.Server.Simulation;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
public sealed class OpcMethodArgumentsAttribute<TEntity, TInterface> : Attribute, IOpcMethodArgumentsAttribute
{
    public OpcMethodArgumentsAttribute(string methodName)
    {
        Entity = typeof(TEntity);
        Interface = typeof(TInterface);
        MethodName = methodName;
        Method = Interface.GetMember(methodName)
            .OfType<MethodInfo>()
            .First();
    }

    public MethodInfo Method { get; }

    public Type Entity { get; }
    public Type Interface { get; }
    public string MethodName { get; }
}