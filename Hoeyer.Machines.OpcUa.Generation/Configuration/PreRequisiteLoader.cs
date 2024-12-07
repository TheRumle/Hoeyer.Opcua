using Hoeyer.Machines.OpcUa.Generated.Configuration;
using Hoeyer.Machines.OpcUa.ResourceLoading;
using Microsoft.CodeAnalysis;

namespace Hoeyer.Machines.OpcUa.Configuration;

[Generator]
internal class PreRequisiteLoader : IIncrementalGenerator
{
    private readonly EmbeddedResourceLoader _loader = new(nameof(OpcNodeConfigurationAttribute), new ResourceMatcher(".cs"));

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

        context.RegisterPostInitializationOutput(postInitializationContext =>
        {
            foreach (LoadableType kvp in _loader.LoadResources())
                postInitializationContext.AddSource($"{kvp.FileName}", kvp.TypeDefinition);
        });
    }
}