using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Generation.IncrementalProvider;

internal static class IncrementalGeneratorExtensions
{
    public static UnloadedIncrementalValuesProvider<TypeContext> GetTypeContextForOpcEntities(
        this IncrementalGeneratorInitializationContext context)
    {
        var valueProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Hoeyer.OpcUa.Core.OpcUaEntityAttribute",
                (_, _) => true,
                (attributeSyntaxContext, cancellationToken) =>
                {
                    if (attributeSyntaxContext.TargetNode is ClassDeclarationSyntax classDeclarationSyntax)
                        return new TypeContext(attributeSyntaxContext.SemanticModel, classDeclarationSyntax);

                    if (attributeSyntaxContext.TargetNode is RecordDeclarationSyntax recordDeclarationSyntax)
                        return new TypeContext(attributeSyntaxContext.SemanticModel, recordDeclarationSyntax);

                    throw new ArgumentException("The attribute is used on something that is not a type declaration");
                })
            .Select((e, c) => new TypeContext(e.SemanticModel, e.Node));

        return new UnloadedIncrementalValuesProvider<TypeContext>(valueProvider);
    }
}