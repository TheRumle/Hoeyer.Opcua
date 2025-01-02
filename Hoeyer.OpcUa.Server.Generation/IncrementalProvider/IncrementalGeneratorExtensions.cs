using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.Generation.IncrementalProvider;

internal static class IncrementalGeneratorExtensions
{
    public static UnloadedIncrementalValuesProvider<TypeContext<T>> GetTypeDeclarationsDecoratedWith<T>(this IncrementalGeneratorInitializationContext context, string attributeMetaName)
        where T : BaseTypeDeclarationSyntax
    {
        var valueProvider = context.SyntaxProvider.ForAttributeWithMetadataName(attributeMetaName,
            predicate: (decoratedClass, cancellationToken) => decoratedClass is T,
            transform: (attributeSyntaxContext, cancellationToken) =>
                new TypeContext<T>(attributeSyntaxContext.SemanticModel, (T)attributeSyntaxContext.TargetNode));
        return new UnloadedIncrementalValuesProvider<TypeContext<T>>(valueProvider);

    }
}