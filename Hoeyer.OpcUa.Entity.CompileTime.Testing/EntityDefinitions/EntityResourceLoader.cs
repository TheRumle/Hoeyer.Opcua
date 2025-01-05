using System.Reflection;
using FluentResults;
using Hoeyer.Common.Extensions.Functional;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public static class EntityResourceLoader
{
    /// <summary>
    /// Loads a resource by mapping the fully qualified name of the type to a path to a file ending with <paramref name="fileType"/>. 
    /// </summary>
    /// <param name="type">The type to be loaded as a resource</param>
    /// <param name="fileType">The file extension of the type. Default is .cs</param>
    /// <returns>A string representing the full source code of the class file.</returns>
    /// <exception cref="FileNotFoundException">If no resource matching the type could be found.</exception>
    public static async Task<Result<string>> LoadTypeAsResourceAsync(Type type, string fileType = ".cs")
    {
        var fileName = type.FullName!.Replace('.', Path.DirectorySeparatorChar) + fileType;
        return await AppDomain
            .CurrentDomain
            .GetAssemblies()
            .Select(async assembly => await ReadResourceAsync(assembly, fileName)).First();
    }

    private static async Task<Result<string>> ReadResourceAsync(Assembly assembly, string resourceName)
    {
        return await assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
            .FailIf(resource => resource == default, $"Could not find resource {resourceName} associated." )
            .Map(async resource =>
            {
                await using var stream = assembly.GetManifestResourceStream(resource!)!;
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            })
            .Traverse();
    }
}