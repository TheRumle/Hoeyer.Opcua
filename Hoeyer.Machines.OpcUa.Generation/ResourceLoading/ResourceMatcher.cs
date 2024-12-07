using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Hoeyer.Machines.OpcUa.ResourceLoading;

internal class ResourceMatcher(string fileType)
{
    public readonly string FileType = fileType;
    private readonly string _fileType = fileType[0] == '.' ? fileType.Substring(1) : fileType;
    internal IEnumerable<(string Resource, string ResourceLocation)> GetAvailableResources(
        List<string> wantedResources,
        string[] loadedResources,
        Action<Diagnostic>? errorReport)
    {
        return GetAvailableResourcesBy(
                wantedResources, (e => e), loadedResources, errorReport)
            .Select(e => (e.Element, e.request.ResourceLocation));
    }


    internal IEnumerable<(T Element, LoadTypeRequest request)> GetAvailableResourcesBy<T>(
        IEnumerable<T> elements,
        Func<T, string> wantedResourceSelector,
        string[] loadedResources,
        Action<Diagnostic>? errorReport)
    {
        var resourceElement = elements.ToDictionary(wantedResourceSelector, e=>e);
        var result = FindAvailableResourceLocation(resourceElement.Keys, loadedResources, _fileType)
            .Select(loadTypeRequest=>(
                elements: resourceElement[loadTypeRequest.WantedResource],
                e: loadTypeRequest)
            ).ToArray();
        
        
        if (errorReport is not null)
        {
            ReportDiagnostics(errorReport, result);
        }

        return result.Where(e=>ResourceAndLocationValid(e.e));
    }

    private static void ReportDiagnostics<T>(Action<Diagnostic> errorReport, (T, LoadTypeRequest request)[] result)
    {
        var missingResources = result
            .Where(e=>!ResourceAndLocationValid(e.request))
            .Select(e => DiagnosticFactory.UnavailableResourceDiagnostic(e.request.WantedResource));

        foreach (var diagnostic in missingResources)
        {
            errorReport.Invoke(diagnostic);
        }
    }


    private static IEnumerable<LoadTypeRequest> FindAvailableResourceLocation(IEnumerable<string> wantedResources, string[] assemblyResources, string fileType)
    {
        return wantedResources.Select(wantedResourceName =>
            new LoadTypeRequest(wantedResourceName, FindResourceOrNull(wantedResourceName, assemblyResources, fileType)));
    }

    private static string FindResourceOrNull(string wantedResourceName, string[] assemblyResources, string fileType)
    {
        return Array.Find(assemblyResources,
            availableResource => availableResource.EndsWith(wantedResourceName + $".{fileType}"));
    }
    
    private static bool ResourceAndLocationValid(LoadTypeRequest arg) => !string.IsNullOrWhiteSpace(arg.WantedResource) && !string.IsNullOrWhiteSpace(arg.ResourceLocation);
}