using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.CodeLoading;

public static class AssemblyLoader
{
    public static readonly IEnumerable<Assembly> CoreAssemblies =
    [
        Assembly.Load("mscorlib"),
        Assembly.Load("netstandard"),
        Assembly.Load("System"),
    ];
    
    public static readonly IReadOnlySet<MetadataReference> CoreMetadataReferences = CoreAssemblies
        .Select(MetadataReference (e) => MetadataReference.CreateFromFile(e.Location))
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
    
    public static IEnumerable<PortableExecutableReference> GetMetaReferencesContainedIn(Type type)
    {

        return GetAssembliesContainedIn(type)
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location)).Union([MetadataReference.CreateFromFile(type.GetTypeInfo().Assembly.Location)]);
    }
    
    public static HashSet<Assembly> GetAssembliesContainedIn(Type type)
    {
        List<Type> otherTypes = [..type.GenericTypeArguments, ..type.CustomAttributes.Select(e=>e.AttributeType)];
        if (type.BaseType != null) otherTypes.Add(type.BaseType);

        return MemberAssemblies(type)
            .Union(otherTypes.AsParallel())
            .Select(e => e.Assembly)
            .ToHashSet();
    }

    
    public static HashSet<Assembly> AssembliesAndCoreAssembliesFor(Type type)
    {
        return AssemblyLoader.GetAssembliesContainedIn(type).Union(CoreAssemblies).ToHashSet();
    }
    
    public static HashSet<MetadataReference> MetaDataReferencesForCoreAnd(Type type)
    {
        return AssemblyLoader.GetMetaReferencesContainedIn(type).Union(CoreMetadataReferences).ToHashSet();
    }




}