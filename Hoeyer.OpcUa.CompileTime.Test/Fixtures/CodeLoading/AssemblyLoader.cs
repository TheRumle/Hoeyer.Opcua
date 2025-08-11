using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Simulation.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Microsoft.CodeAnalysis;
using Assembly = System.Reflection.Assembly;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.CodeLoading;

public static class AssemblyLoader
{
    public static readonly HashSet<Assembly> CoreAssemblies =
    [
        Assembly.Load("mscorlib"),
        Assembly.Load("netstandard"),
        Assembly.Load("System"),
        Assembly.Load("System.Runtime"),
        typeof(OpcUaAgentAttribute).Assembly,
        typeof(OpcUaAgentMethodsAttribute<>).Assembly,
        typeof(OpcMethodArgumentsAttribute<,>).Assembly,
        typeof(AgentServerStartedMarker).Assembly,
        typeof(ISimulation<,>).Assembly,
    ];

    public static readonly IReadOnlySet<MetadataReference> CoreMetadataReferences = CoreAssemblies
        .Select(MetadataReference (e) => MetadataReference.CreateFromFile(e.Location))
        .Union([MetadataReference.CreateFromFile(typeof(object).Assembly.Location)])
        .ToHashSet();

    private static ParallelQuery<Type> MemberAssemblies(Type type)
    {
        return type
            .GetMembers() // Includes methods, properties, fields, etc.
            .AsParallel()
            .SelectMany(m => m.GetCustomAttributesData().SelectMany(a => new HashSet<Type>([
                    a.AttributeType,
                    ..a.ConstructorArguments.Select(arg => arg.ArgumentType)
                ]))
            );
    }

    public static HashSet<Assembly> GetAssembliesContainedIn(Type type)
    {
        List<Type> otherTypes = [..type.GenericTypeArguments, ..type.CustomAttributes.Select(e => e.AttributeType)];
        if (type.BaseType != null)
        {
            otherTypes.Add(type.BaseType);
        }

        return MemberAssemblies(type)
            .Union(otherTypes.AsParallel())
            .Select(e => e.Assembly)
            .ToHashSet();
    }
}