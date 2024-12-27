using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Hoeyer.Machines.OpcUa.ResourceLoading;

internal class AssociativeEmbeddedResourceLoader<T>(
    List<(string resourceName, T associatedResource) > resourceNames,
    ResourceMatcher matcher,
    Func<T, string> generatedFileName,
    Action<Diagnostic>? errorReport = null) : IResourceLoader
{

    private IEnumerable<LoadableType>? _values;

    
    /// <inheritdoc />
    public IEnumerable<LoadableType> LoadResources() => GetValue();

    private IEnumerable<LoadableType> GetValue()
    {
        if (_values is not null) return _values;
        _values = LoadEmbeddedFile(resourceNames, errorReport, matcher, generatedFileName).Result;
        return _values;
    }

    private static async Task<IEnumerable<LoadableType>> LoadEmbeddedFile(List<(string resourceName, T associatedResource)> wantedResources, Action<Diagnostic>? errorReport, ResourceMatcher matcher, Func<T, string> resourceToFileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyResources = assembly.GetManifestResourceNames();
        var resourceAndName = wantedResources.ToDictionary(e => e.associatedResource, e=>e.resourceName);
        
        var availableResources = matcher.GetAvailableResourcesBy(
            resourceAndName.Keys,
            e=>resourceAndName[e],
            assemblyResources,
            errorReport);

        return await AssemblyResourceLoader.LoadAssemblyTypes(
            availableResources,
            assembly,
            e=>resourceToFileName.Invoke(e.Element), 
            e=>e.request.ResourceLocation
        );

    }

}