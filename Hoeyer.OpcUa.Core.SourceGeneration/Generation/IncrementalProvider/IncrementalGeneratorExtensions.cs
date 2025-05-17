using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Generation.IncrementalProvider;

internal static class IncrementalGeneratorExtensions
{
    public static UnloadedIncrementalValuesProvider<TypeContext<T>> GetTypeContextForOpcEntities<T>(
        this IncrementalGeneratorInitializationContext context)
        where T : TypeDeclarationSyntax
    {
        var valueProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Hoeyer.OpcUa.Core.OpcUaEntityAttribute",
                (_, _) => true,
                (attributeSyntaxContext, cancellationToken) => new TypeContext<T>(attributeSyntaxContext.SemanticModel,
                    (T)attributeSyntaxContext.TargetNode))
            .Select((e, c) => new TypeContext<T>(e.SemanticModel, e.Node));

        return new UnloadedIncrementalValuesProvider<TypeContext<T>>(valueProvider);
    }
}