using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hoeyer.Machines.OpcUa.ResourceLoading;

internal static class AssemblyResourceLoader
{
    private static async Task<string> ReadResourceStreamAsync(string resourceLocation, Assembly assembly)
    {
        var stream = assembly.GetManifestResourceStream(resourceLocation)!;
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    public static async Task<IEnumerable<LoadableType>> LoadAssemblyTypes<T>(
        IEnumerable<T> availableResources,
        Assembly assembly,
        Func<T, string> resourceToFileName,
        Func<T, string> assemblyResourceName)
    {
        var loadableTypes = availableResources.Select(
                async resource => await LoadFromAssembly<T>(assembly, resourceToFileName.Invoke(resource), assemblyResourceName.Invoke(resource)))
            .ToArray();

        await Task.WhenAll(loadableTypes);
        return loadableTypes.Select(value => value.Result);
    }

    private static async Task<LoadableType> LoadFromAssembly<T>(Assembly assembly,
        string resourceLocationName,
        string assemblyResourceName)
    {
        return new LoadableType(await ReadResourceStreamAsync(assemblyResourceName, assembly), resourceLocationName);
    }
}