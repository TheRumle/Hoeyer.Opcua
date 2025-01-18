using System.Collections.Immutable;
using System.Reflection;
using FluentResults;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public static class OpcEntityResourceLoader
{
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

    private static readonly ImmutableSortedDictionary<string, Lazy<string>> AssemblyResources = Assembly
        .GetManifestResourceNames()
        .ToImmutableSortedDictionary(
            resource => resource,
            resource => new Lazy<string>(() =>
                new StreamReader(Assembly.GetManifestResourceStream(resource)!).ReadToEnd())
        );

    private static ImmutableDictionary<Type, string> Resources => Assembly.GetExportedTypes()
        .Where(type => AssemblyResources.ContainsKey(type.FullName + ".cs"))
        .ToImmutableDictionary(
            type => type,
            type => AssemblyResources[type.FullName + ".cs"].Value
        );


    /// <summary>
    ///     Loads a resource by mapping the fully qualified name of the type to a path to a file ending with
    ///     <paramref name="fileType" />.
    /// </summary>
    /// <param name="type">The type to be loaded as a resource</param>
    /// <returns>A string representing the full source code of the class file.</returns>
    /// <exception cref="FileNotFoundException">If no resource matching the type could be found.</exception>
    public static Result<(Type Type, string TypeAsString)> LoadTypeAsResourceString(Type type)
    {
        var resourceName = type.FullName! + ".cs";
        if (Resources.TryGetValue(type, out var resource)) return Result.Ok((type, resource));
        return Result.Fail("Resource not found: " + resourceName);
    }
}