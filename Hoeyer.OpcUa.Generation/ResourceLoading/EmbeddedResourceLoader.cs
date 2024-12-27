using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Hoeyer.Machines.OpcUa.ResourceLoading;

internal class EmbeddedResourceLoader(List<string> resourceNames, ResourceMatcher matcher, Action<Diagnostic>? errorReport = null) : IResourceLoader
{
    public EmbeddedResourceLoader(string resourceName, ResourceMatcher matcher, Action<Diagnostic>? errorReport = null)
    :this([resourceName], matcher, errorReport)
    {
        
    }
    private readonly IEnumerable<LoadableType> _wantedResources = LoadEmbeddedFile(resourceNames, errorReport, matcher).Result;
    public IEnumerable<LoadableType> LoadResources() => _wantedResources;

    private static async Task<IEnumerable<LoadableType>> LoadEmbeddedFile(List<string> wantedResources,
        Action<Diagnostic>? errorReport, ResourceMatcher matcher)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyResources = assembly.GetManifestResourceNames();
        var availableResources = matcher.GetAvailableResources(wantedResources, assemblyResources, errorReport);

        return await AssemblyResourceLoader.LoadAssemblyTypes(availableResources, assembly,
            resource => resource.Resource + $".g.{matcher.FileType}", e => e.ResourceLocation);
    }
}