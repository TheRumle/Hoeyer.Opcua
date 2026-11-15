using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Generation;

public static class GeneratorInitializationContextExtensions
{
    public static UnloadedIncrementalValuesProvider<TypeContext> GetEntityMethodSyntaxes(
        this IncrementalGeneratorInitializationContext context)
    {
        var valueProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                WellKnown.FullyQualifiedAttribute.EntityBehaviourAttributeDef.WithoutGlobalPrefix,
                (_, _) => true,
                (attributeSyntaxContext, cancellationToken) =>
                {
                    if (attributeSyntaxContext.TargetNode is InterfaceDeclarationSyntax interfaceSyntax)
                    {
                        return new TypeContext(attributeSyntaxContext.SemanticModel, interfaceSyntax);
                    }
                    throw new ArgumentException("The attribute is used on something that is not a interface syntax");
                })
            .Select((e, c) => new TypeContext(e.SemanticModel, e.Node));

        return new UnloadedIncrementalValuesProvider<TypeContext>(valueProvider);
    }
}