using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Assembly = System.Reflection.Assembly;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.CodeLoading;

public static class AssemblyLoader
{
    public static readonly IEnumerable<Assembly> CoreAssemblies =
    [
        Assembly.Load("mscorlib"),
        Assembly.Load("netstandard"),
        Assembly.Load("System"),
        typeof(Hoeyer.OpcUa.Core.OpcUaEntityAttribute).Assembly,
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