using Hoeyer.OpcUa.EntityGeneration.IncrementalProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Generation.IncrementalProvider;

internal static class IncrementalGeneratorExtensions
{
    public static UnloadedIncrementalValuesProvider<TypeContext<T>> GetTypeDeclarationsDecoratedWith<T>(
        this IncrementalGeneratorInitializationContext context, string attributeMetaName)
        where T : BaseTypeDeclarationSyntax
    {
        var valueProvider = context.SyntaxProvider.ForAttributeWithMetadataName(attributeMetaName,
            (decoratedClass, cancellationToken) => decoratedClass is T,
            (attributeSyntaxContext, cancellationToken) =>
                new TypeContext<T>(attributeSyntaxContext.SemanticModel, (T)attributeSyntaxContext.TargetNode));
        return new UnloadedIncrementalValuesProvider<TypeContext<T>>(valueProvider);
    }
}