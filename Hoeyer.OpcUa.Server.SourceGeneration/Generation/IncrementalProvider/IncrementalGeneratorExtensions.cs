using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation.IncrementalProvider;

internal static class IncrementalGeneratorExtensions
{
    public static UnloadedIncrementalValuesProvider<TypeContext<T>> GetTypeDeclarationsDecoratedWith<T>(
        this IncrementalGeneratorInitializationContext context, string attributeMetaName)
        where T : TypeDeclarationSyntax
    {
        var valueProvider = context.SyntaxProvider.ForAttributeWithMetadataName(attributeMetaName,
            predicate: (_,_) => true,
            (attributeSyntaxContext, cancellationToken) =>
                new TypeContext<T>(attributeSyntaxContext.SemanticModel, (T)attributeSyntaxContext.TargetNode));
        return new UnloadedIncrementalValuesProvider<TypeContext<T>>(valueProvider);
    }
}