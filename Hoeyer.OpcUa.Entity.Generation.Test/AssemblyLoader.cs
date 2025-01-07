using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Entity.Generation.Test;

internal static class AssemblyLoader
{
    private static IEnumerable<Assembly> CoreAssemblies =
    [
        Assembly.Load("mscorlib"),
        Assembly.Load("netstandard"),
        Assembly.Load("System"),
    ];
    
    public static readonly IReadOnlySet<MetadataReference> BaseReferences = CoreAssemblies
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
        List<Type> otherTypes = [..type.GenericTypeArguments, ..type.CustomAttributes.Select(e=>e.AttributeType)];
        if (type.BaseType != null) otherTypes.Add(type.BaseType);
        
        return MemberAssemblies(type)
            .Union(otherTypes.AsParallel())
            .ToHashSet()
            .Select(t => MetadataReference.CreateFromFile(t.Assembly.Location));
    }




}