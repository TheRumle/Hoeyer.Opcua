using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Generation.IncrementalProvider;

internal static class IncrementalGeneratorExtensions
{
    public static UnloadedIncrementalValuesProvider<TypeContext<InterfaceDeclarationSyntax>> GetEntityMethodInterfaces(
        this IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<TypeContext<InterfaceDeclarationSyntax>> valueProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Hoeyer.OpcUa.Core.OpcUaEntityMethodsAttribute",
                (_, _) => true,
                (attributeSyntaxContext, cancellationToken) => new TypeContext<InterfaceDeclarationSyntax>(
                    attributeSyntaxContext.SemanticModel,
                    (InterfaceDeclarationSyntax)attributeSyntaxContext.TargetNode))
            .Select((e, c) => new TypeContext<InterfaceDeclarationSyntax>(e.SemanticModel, e.Node));

        return new UnloadedIncrementalValuesProvider<TypeContext<InterfaceDeclarationSyntax>>(valueProvider);
    }
}