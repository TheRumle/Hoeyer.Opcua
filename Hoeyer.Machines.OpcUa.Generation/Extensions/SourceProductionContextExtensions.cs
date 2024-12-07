using Hoeyer.Machines.OpcUa.ResourceLoading;
using Microsoft.CodeAnalysis;

namespace Hoeyer.Machines.OpcUa.Extensions;

internal static class SourceProductionContextExtensions
{
    public static void Load(this SourceProductionContext context, LoadableType type)
    {
        context.AddSource(type.FileName, type.TypeDefinition);
    }
}