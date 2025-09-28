using System;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Core.Application.NodeStructureFactory;

namespace Hoeyer.OpcUa.Simulation.Api;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
public sealed class OpcMethodArgumentsAttribute<TEntity, TInterface> : Attribute, IOpcMethodArgumentsAttribute
{
    public OpcMethodArgumentsAttribute(string methodName)
    {
        Entity = typeof(TEntity);
        Interface = typeof(TInterface);
        Method = Interface.GetMember(methodName)
            .OfType<MethodInfo>()
            .First();
        MethodName = Method.GetBrowseNameOrDefault(methodName);
    }

    public MethodInfo Method { get; }

    public Type Entity { get; }
    public Type Interface { get; }
    public string MethodName { get; }
}